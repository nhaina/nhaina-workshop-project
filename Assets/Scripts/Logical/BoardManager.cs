using System;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public class BoardManager : IBoardManager
	{
		private BoardState current;

		public BoardState Current
		{
			get
			{
				return current;
			}
			set
			{
				current = value;
				AfterStateUpdate?.Invoke(current, GameState);
			}
		}
		public eGameState GameState { get; private set; }
		public event Action<BoardState, eGameState> AfterStateUpdate;

		public void CreateNew()
		{
			Current = new BoardState();
			GameState = eGameState.Setup;
		}

		public void EndTurn()
		{
			ePlayer activePlayer = Current.ActivePlayer == ePlayer.Player1 ? ePlayer.Player2 : ePlayer.Player1;
			BoardState newState = new BoardState(Current.Bank, Current.Stars, activePlayer);

			changeGameState(newState);
			Current = newState;
		}

		public void UpdateResources(ResourcesState i_NewState)
		{
			Current = new BoardState(i_NewState, Current.Stars,
				Current.ActivePlayer);
		}

		public void SetupHomeWorld(HomeWorld i_HomeWorld)
		{
			BoardState newState = new BoardState(
				Current.Bank,
				new List<Star>(Current.Stars) { i_HomeWorld },
				Current.ActivePlayer);

			if (newState.Player1Star != null && newState.Player2Star != null)
			{
				GameState = eGameState.Running;
			}

			Current = newState;
		}

		public void UpdateStars(IEnumerable<Star> i_NewStarsState)
		{
			if (GameState != eGameState.Running)
			{
				bool isEnded = GameState != eGameState.Setup;
				throw new InvalidOperationException($"Attempt to play {(isEnded ? "after game ended!" : "in an uninitialized game!")}");
			}

			Current = new BoardState(Current.Bank, i_NewStarsState, Current.ActivePlayer);
		}

		private void changeGameState(BoardState newState)
		{
			if (GameState == eGameState.Running)
			{
				bool player1Loss = newState.Player1Star == null || newState.Player1Star.IsEmpty;
				bool player2Loss = newState.Player2Star == null || newState.Player2Star.IsEmpty;
				if (player1Loss && player2Loss)
				{
					GameState = eGameState.Draw;
				}
				else if (newState.Player1Star == null || newState.Player1Star.IsEmpty)
				{
					GameState = eGameState.Player2Won;
				}
				else if (newState.Player2Star == null || newState.Player2Star.IsEmpty)
				{
					GameState = eGameState.Player1Won;
				}
			}
		}
	}
}
