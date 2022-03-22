using Homeworlds.Common;
using System;

namespace Homeworlds.Logic
{
	public class GameManager
	{
		public event Action<BoardState> BoardUpdated;
		public event Action<ePlayer> TurnEnded;

		public GameManager()
		{

		}

		public BoardManager BoardManager { get; set; }

		public int ActionsCounter { get; private set; }

		public ePlayer ActivePlayer { private set; get; }

		protected virtual void OnBoardUpdated()
		{
			BoardUpdated?.Invoke(BoardManager.CurrentState);
		}

		protected virtual void OnTurnEnded()
		{
			TurnEnded?.Invoke(ActivePlayer);
		}
	}
}