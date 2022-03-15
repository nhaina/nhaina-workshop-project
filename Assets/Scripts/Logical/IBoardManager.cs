using System;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public interface IBoardManager
	{
		eGameState GameState { get; }
		BoardState Current { get; }
		event Action<BoardState, eGameState> AfterStateUpdate;

		void CreateNew();
		void EndTurn();
		void UpdateResources(ResourcesState i_NewState);
		void SetupHomeWorld(HomeWorld i_HomeWorld);
		void UpdateStars(IEnumerable<Star> i_NewStarsState);
	}
}