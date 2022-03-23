using Homeworlds.Common;
using Homeworlds.Logic;
using Homeworlds.View;
using System;

namespace Homeworlds.Controller
{
	public class MainController
	{
		public event Action<BoardState> BoardUpdated;
		public event Action<ePlayer> TurnEnded;
		public event Action<eBoardLifecycle> GameEnded;

		private ISelectable initiator;

		public MainController()
		{
			initiator = null;
		}


		public ViewBoard View { get; set; }
		public GUIManager GUI { get; set; }
		public BoardManager BoardManager { get; set; }
		public ePlayer ActivePlayer { private set; get; }
		public PlayerController PlayerController { get; set; }

		protected virtual void OnBoardUpdated()
		{
			BoardUpdated?.Invoke(BoardManager.CurrentState);
			View.UpdateField(BoardManager.CurrentState, BoardManager.BankState);
		}

		protected virtual void OnTurnEnded()
		{
			TurnEnded?.Invoke(ActivePlayer);
			checkBoardState();
		}

		protected virtual void OnGameEnded(eBoardLifecycle boardStatus)
		{
			GameEnded?.Invoke(boardStatus);
		}

		private void checkBoardState()
		{
			BoardManager.CheckBoardStatus();
			eBoardLifecycle boardStatus = BoardManager.CurrentState.Status;
			if (boardStatus == eBoardLifecycle.Player1Won ||
				boardStatus == eBoardLifecycle.Player2Won ||
				boardStatus == eBoardLifecycle.Draw)
			{
				OnGameEnded(boardStatus);
			}
		}

		public void StartGame()
		{
			GUI.PopSelection(Utilities.AllPips, "Player1, Select Your Homeworld:",
				i_SelectionNeeded: 2, i_MaxControlsInRow: 4);
			GUI.PopSelection(Utilities.AllOf(ePipSize.Large), "Player1, Select Your Ship:");

			GUI.PopSelection(Utilities.AllPips, "Player2, Select Your Homeworld:",
				i_SelectionNeeded: 2, i_MaxControlsInRow: 4);
			GUI.PopSelection(Utilities.AllOf(ePipSize.Large), "Player2, Select Your Ship:");
		}

		private void onSelectedShip(ISelectable selectedShip)
		{
		}

		private void onSelectedStar(ISelectable selectedStar)
		{

		}
	}
}