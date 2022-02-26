using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public interface IShipContainer
	{
		void AddShip(Ship ship);
		void RemoveShip(Ship ship);
		bool IsEmpty { get; }

		IEnumerable<ePipColor> GetOverpopulatedColors();
		int GetColorCount(ePipColor color);
	}
}