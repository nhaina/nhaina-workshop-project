using System;
using System.Collections.Generic;
using System.Linq;
using Homeworlds.Common;
using Homeworlds.Logic;
using UnityEngine; // for Mathf.min

namespace Homeworlds.AI
{
	class AlphaBetaCutoffMinimax : IAiStrategy
	{
		public Func<BoardState, int, bool> CutoffTest { get; set; }
		public Func<BoardState, float> Eval { get; set; }
		public BoardManager GameBoard { get; set; }

		public AlphaBetaCutoffMinimax()
		{ }

		private static bool DefaultCutoffTest(BoardState state, int depth)
		{
			return state.Status == eBoardLifecycle.Player1Won || state.Status == eBoardLifecycle.Player2Won ||
				state.Status == eBoardLifecycle.Draw || depth > 2;
		}

		private static Func<BoardState, float> DefaultEvalWrapper(ePlayer activePlayer)
		{
			return Eval;

			float Eval(BoardState state)
			{
				float score, sign = 1f;
				switch (state.Status)
				{
					case eBoardLifecycle.Player1Won:
					case eBoardLifecycle.Player2Won:
						score = 100;
						sign = activePlayer == ePlayer.Player1 ^ state.Status == eBoardLifecycle.Player1Won ? -1 : 1;
						break;
					case eBoardLifecycle.Draw:
						score = 10;
						break;
					default:
						score = state.Ships.Where(ship => ship.Owner == activePlayer).Count() - state.Ships.Where(ship => ship.Owner != activePlayer).Count();
						break;
				}
				return score * sign;
			}
		}

		public IBoardMove GetMove()
		{
			BoardState state = GameBoard.CurrentState;
			ePlayer me = state.ActivePlayer;
			CutoffTest = CutoffTest is null ? DefaultCutoffTest : CutoffTest;
			Eval = Eval is null ? DefaultEvalWrapper(state.ActivePlayer) : Eval;
			List<IBoardMove> bestMoves = new List<IBoardMove>();
			float bestScore = float.NegativeInfinity;

			foreach (IBoardMove move in GameBoard.GetAllAvailableMoves(me))
			{
				if (move is SacrificeMove)
				{
					continue;
				}
				GameBoard.CurrentState = state.Clone();
				move.Execute();
				float score;
				if (GameBoard.ActivePlayer == me)
				{
					score = MaxValue(GameBoard.CurrentState, bestScore, float.PositiveInfinity, 1);
				}
				else
				{
					score = MinValue(GameBoard.CurrentState, bestScore, float.PositiveInfinity, 1);
				}

				if (score > bestScore)
				{
					bestScore = score;
					bestMoves.Clear();
					bestMoves.Add(move);
				}
				else if (score == bestScore)
				{
					bestMoves.Add(move);
				}
			}

			IBoardMove chosen = bestMoves.Count == 0 ? null : bestMoves[RandomGenerator.Random.Next(bestMoves.Count)];
			return chosen;
		}

		private float MinValue(BoardState state, float alpha, float beta, int depth)
		{
			float score;
			ePlayer active = state.ActivePlayer;
			IEnumerable<IBoardMove> boardMoves = GameBoard.GetAllAvailableMoves(active);
			if (boardMoves.Count() == 0)
			{
				GameBoard.EndTurn();
			}

			if (CutoffTest(state, depth))
			{
				score = Eval(state);
			}
			else
			{
				score = float.PositiveInfinity;
				foreach (IBoardMove move in boardMoves)
				{
					if (move is SacrificeMove)
					{
						continue;
					}
					GameBoard.CurrentState = state.Clone();
					move.Execute();
					if (active == GameBoard.CurrentState.ActivePlayer)
					{
						score = Mathf.Min(score, MinValue(GameBoard.CurrentState, alpha, beta, depth + 1));
					}
					else
					{
						score = Mathf.Min(score, MaxValue(GameBoard.CurrentState, alpha, beta, depth + 1));
					}
					if (score <= alpha)
					{
						break;
					}
					beta = Mathf.Min(beta, score);
				}
			}
			return score;
		}

		private float MaxValue(BoardState state, float alpha, float beta, int depth)
		{
			float score;
			ePlayer active = state.ActivePlayer;
			IEnumerable<IBoardMove> boardMoves = GameBoard.GetAllAvailableMoves(active);
			if (boardMoves.Count() == 0)
			{
				GameBoard.EndTurn();
			}

			if (CutoffTest(state, depth))
			{
				score = Eval(state);
			}
			else
			{
				score = float.NegativeInfinity;
				foreach (IBoardMove move in boardMoves)
				{
					if (move is SacrificeMove)
					{
						continue;
					}
					GameBoard.CurrentState = state.Clone();
					move.Execute();
					if (active == GameBoard.CurrentState.ActivePlayer)
					{
						score = Mathf.Max(score, MaxValue(GameBoard.CurrentState, alpha, beta, depth + 1)); 
					}
					else
					{
						score = Mathf.Max(score, MinValue(GameBoard.CurrentState, alpha, beta, depth + 1));
					}
					if (score >= beta)
					{
						break;
					}
					alpha = Mathf.Max(alpha, score);
				}
			}
			return score;
		}
	}
}
