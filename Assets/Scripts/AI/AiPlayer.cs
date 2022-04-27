using Homeworlds.Common;
using Homeworlds.Logic;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Homeworlds.AI
{
	public class AiPlayer
	{
		public IAiStrategy Strategy { get; set; }
		public ePlayer Player { get; set; }
		public BoardManager GameBoard
		{
			get
			{
				return gameBoard;
			}
			set
			{
				if (gameBoard != null)
				{
					gameBoard.TurnEnded -= GameBoard_TurnEnded;
					gameBoard.BoardFinishSetup -= GameBoard_TurnEnded;
				}
				gameBoard = value;
				if (gameBoard != null)
				{
					if (gameBoard.CurrentState.Status == eBoardLifecycle.Setup)
					{
						gameBoard.BoardFinishSetup += copyBoard;
					}
				}
			}
		}

		private void copyBoard()
		{
			aiGameBoard = new BoardManager();
			aiGameBoard.MovesFactory = gameBoard.MovesFactory;
			aiGameBoard.CurrentState = gameBoard.CurrentState.Clone();
			aiGameBoard.ActivePlayer = gameBoard.ActivePlayer;
			Strategy.GameBoard = aiGameBoard;
			gameBoard.TurnEnded += GameBoard_TurnEnded;
		}

		private void GameBoard_TurnEnded()
		{
			if (Player == GameBoard.ActivePlayer)
			{
				DoMove();
			}
		}

		private BoardManager gameBoard, aiGameBoard;

		public AiPlayer()
		{ }

		async void DoMove()
		{
			aiGameBoard.CurrentState = GameBoard.CurrentState.Clone();
			float time = Time.realtimeSinceStartup;
			while (GameBoard.ActivePlayer == Player && GameBoard.BoardStatus == eBoardLifecycle.Ongoing)
			{
				IBoardMove boardMove = await Task.Run(Strategy.GetMove);
				if (boardMove != null)
				{
					boardMove.BoardManager = GameBoard;
					Debug.Log($"move: {boardMove}, activePlayer: {GameBoard.ActivePlayer}");
					boardMove.Execute();
				}
			}
			Debug.Log($"{1000* (Time.realtimeSinceStartup - time)} ms");
		}
	}
}
