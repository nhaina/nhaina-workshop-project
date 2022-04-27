using Homeworlds.Common;
using Homeworlds.Logic;
using Homeworlds.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Homeworlds.Controller
{
	public class MainController
	{
		public event Action<BoardState> BoardUpdated;

		private ViewBoard viewBoard;
		private BoardManager boardManager;
		private Homeworld? p1Hw, p2Hw;
		private Ship? p1Ship, p2Ship;
		private Pip? lastSelected;

		public MainController()
		{ }


		public ViewBoard View
		{
			get
			{
				return viewBoard;
			}
			set
			{
				if (viewBoard != null)
				{
					viewBoard.SelectShipCallback = null;
					viewBoard.SelectStarCallback = null;
				}
				viewBoard = value;
				if (viewBoard != null)
				{
					viewBoard.SelectShipCallback = onSelectedShip;
					viewBoard.SelectStarCallback = onSelectedStar;
				}
			}
		}

		public BoardManager BoardManager
		{
			get
			{
				return boardManager;
			}
			set
			{
				if (boardManager != null)
				{
					boardManager.BoardUpdated -= boardUpdated;
				}
				boardManager = value;
				if (boardManager != null)
				{
					boardManager.BoardUpdated += boardUpdated;
				}
			}
		}


		public GUIManager GUI { get; set; }

		protected virtual void OnBoardUpdated(BoardState i_NewState)
		{
			BoardUpdated?.Invoke(i_NewState);
		}

		private void boardUpdated(BoardState i_NewState)
		{
			updateViewField(i_NewState);
			OnBoardUpdated(i_NewState);
		}

		private void updateViewField(BoardState i_NewState)
		{
			View.UpdateField(i_NewState, BoardManager.BankState);
		}

		public void StartGame()
		{
			GUI.SelectionCallback = GUI_SelectedHomeworldPlayer;
			GUI.PopSelection(Utilities.AllPips.OrderBy(p => p.Size).Select(p => new UIDrawablePip() { Pip = p }), "Player1, Select Your Homeworld:",
				i_MaxControlsInRow: 4);
			BoardManager.StartNewGame();
		}

		private void GUI_SelectedHomeworldPlayer(IUIDrawable i_SelectedPip)
		{
			Pip selectedPip = ((UIDrawablePip)i_SelectedPip).Pip;
			if (lastSelected.HasValue)
			{
				ePlayer active;
				if (p1Hw == null)
				{
					active = ePlayer.Player1;
					p1Hw = new Homeworld(lastSelected.Value, selectedPip, active, false);
				}
				else
				{
					active = ePlayer.Player2;
					p2Hw = new Homeworld(lastSelected.Value, selectedPip, active, false);
				}
				GUI.SelectionCallback = GUI_SelectedShipPlayer;
				GUI.PopSelection(Utilities.AllOf(ePipSize.Large).OrderBy(p => p.Size).Select(p => new UIDrawablePip() { Pip = p }),
					$"Player{(active == ePlayer.Player1 ? 1 : 2)}, Select Your Ship:");
				lastSelected = null;
			}
			else
			{
				lastSelected = selectedPip;
			}
		}

		private void GUI_SelectedShipPlayer(IUIDrawable i_SelectedPip)
		{
			Pip selectedPip = ((UIDrawablePip)i_SelectedPip).Pip;
			if (p1Ship == null)
			{
				p1Ship = new Ship(selectedPip, ePlayer.Player1, p1Hw);
				GUI.SelectionCallback = GUI_SelectedHomeworldPlayer;
				GUI.PopSelection(Utilities.AllPips.OrderBy(p => p.Size).Select(p => new UIDrawablePip() { Pip = p }),
					"Player2, Select Your Homeworld:", i_MaxControlsInRow: 4);
			}
			else
			{
				p2Ship = new Ship(selectedPip, ePlayer.Player2, p2Hw);
				GUI.SelectionCallback = null;
				GUI.CloseUI();
				BoardManager.SetupGame(p1Hw.Value, p1Ship.Value, p2Hw.Value, p2Ship.Value);
			}
		}

		private void onSelectedShip(ISelectable selectedShip)
		{
			Ship ship = ((ShipDescriptor)selectedShip).Ship;
			if (ship.Owner == BoardManager.ActivePlayer)
			{
				IEnumerable<IBoardMove> moves = BoardManager.GetAvailableMoves(ship);
				GUI.SelectionCallback = GUI_SelectedMove;
				GUI.PopSelection(moves.Select(bm => new UIDrawableBoardMove() { GameViewBoard = viewBoard, BoardMove = bm }), $"{ship.Attributes} Ship:");
			}
			else
			{
				GUI.PopInformation(selectedShip);
			}
		}

		private void GUI_SelectedMove(IUIDrawable i_SelectedBoardMove)
		{
			GUI.CloseUI();
			IBoardMove boardMove = ((UIDrawableBoardMove)i_SelectedBoardMove).BoardMove;
			boardMove.Execute();
		}

		private void onSelectedStar(ISelectable selectedStar)
		{
			IStar star = ((StarDescriptor)selectedStar).Star;
			IEnumerable<IBoardMove> moves = BoardManager.CanDeclareCatastrophe(star);
			GUI.SelectionCallback = GUI_SelectedMove;
			GUI.PopSelection(moves.Select(bm => new UIDrawableBoardMove() { GameViewBoard = viewBoard, BoardMove = bm }), $"{viewBoard.GetStarName(star.Identifier)} ({string.Join(" ", star.Attributes)}):");
		}
	}
}