using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.Logical
{
	public class Board
	{
		BoardState current;

		public void CreateNew()
		{
			current = new BoardState();
			BoardState = eBoardState.Setup;
		}

		public void EndTurn()
		{
			ePlayer activePlayer = current.ActivePlayer == ePlayer.Player1 ? ePlayer.Player2 : ePlayer.Player1;

			current = new BoardState(current.Bank, current.Stars,
				activePlayer, 1 + current.StepNumber);
		}

		public void UpdateResources(ResourcesState i_NewState)
		{
			current = new BoardState(i_NewState, current.Stars,
				current.ActivePlayer, current.StepNumber);
		}

		public void SetupHomeWorld(HomeWorld i_HomeWorld)
		{
			int starIdx = current.ActivePlayer == ePlayer.Player1 ? 0 : 1;
			List<Star> stars = current.Stars;
			stars[starIdx] = i_HomeWorld;

			current = new BoardState(current.Bank, stars,
				current.ActivePlayer, 1 + current.StepNumber);

			if(current.Player1Star != null && current.Player2Star != null)
			{
				BoardState = eBoardState.Running;
			}
		}

		public void UpdateStars(List<Star> i_NewStarsState, bool i_IsStep)
		{
			if(BoardState != eBoardState.Running)
			{
				Debug.LogError("Attempt to Play in an Uninitialize Board!");
			}

			int stepNumber = current.StepNumber + (i_IsStep ? 1 : 0);

			current = new BoardState(current.Bank, i_NewStarsState,
				current.ActivePlayer, stepNumber);

			if(i_IsStep)
			{
				checkBoardStateChanged();
			}
		}

		private void checkBoardStateChanged()
		{
			if (BoardState == eBoardState.Running)
			{
				if (current.Player1Star == null || current.Player1Star.IsEmpty)
				{
					BoardState = eBoardState.Player2Won;
				}
				else if (current.Player2Star == null || current.Player2Star.IsEmpty)
				{
					BoardState = eBoardState.Player1Won;
				}
			}
		}

		public BoardState Current
		{
			get { return current; }
		}

		public eBoardState BoardState { get; private set; }
	}
}
