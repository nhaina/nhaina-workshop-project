using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public readonly struct BoardState
	{
		private readonly List<Ship> ships;
		private readonly List<IStar> stars;
		private readonly eBoardLifecycle status;

		private BoardState(List<Ship> i_Ships, List<IStar> i_Stars, eBoardLifecycle i_Status)
		{
			ships = i_Ships;
			stars = i_Stars;
			status = i_Status;
		}

		public BoardState Clone()
		{
			return new BoardState(new List<Ship>(ships), new List<IStar>(stars), status);
		}

		public Ship GetShip(int i_Index)
		{
			return ships[i_Index];
		}

		public IStar GetStar(int i_Index)
		{
			return stars[i_Index];
		}

		public static BoardState AddShip(BoardState i_Original, Ship i_ShipToAdd, out int o_ShipIdx)
		{
			if (i_ShipToAdd.StarIdx < 0 || i_ShipToAdd.StarIdx >= i_Original.stars.Count)
			{
				throw new ArgumentOutOfRangeException("Attempt to add ship to an invalid star index!");
			}

			List<Ship> ships = i_Original.stars == null ? new List<Ship>() : new List<Ship>(i_Original.ships);
			ships.Add(i_ShipToAdd);
			o_ShipIdx = i_Original.ships.Count;

			return new BoardState(ships, new List<IStar>(i_Original.stars), i_Original.status);
		}

		public static BoardState AddStar(BoardState i_Original, IStar i_StarToAdd, out int o_StarIdx)
		{
			List<IStar> stars = i_Original.stars == null ? new List<IStar>() : new List<IStar>(i_Original.stars);
			stars.Add(i_StarToAdd);
			o_StarIdx = i_Original.ships.Count;

			return new BoardState(new List<Ship>(i_Original.ships), stars, i_Original.status);
		}

		public static BoardState RemoveShipAt(BoardState i_Original, int i_ShipIdx)
		{
			if (i_ShipIdx < 0 || i_Original.ships == null || i_ShipIdx >= i_Original.ships.Count)
			{
				throw new ArgumentOutOfRangeException("Attempt to remove ship with invalid index!");
			}

			List<Ship> ships = new List<Ship>(i_Original.ships);
			ships.RemoveAt(i_ShipIdx);
			return new BoardState(ships, new List<IStar>(i_Original.stars), i_Original.status);
		}

		public static BoardState RemoveStarAt(BoardState i_Original, int i_StarIdx)
		{
			if (i_StarIdx < 0 || i_Original.stars == null || i_StarIdx >= i_Original.stars.Count)
			{
				throw new ArgumentOutOfRangeException("Attempt to remove star with invalid index!");
			}

			List<IStar> stars = new List<IStar>(i_Original.stars);
			stars.RemoveAt(i_StarIdx);
			List<Ship> ships = i_Original.ships.Where(ship => ship.StarIdx != i_StarIdx).ToList();
			return new BoardState(ships, stars, i_Original.status);
		}

		public static BoardState ChangeGameStatus(BoardState i_Original, eBoardLifecycle i_NewStatus)
		{
			return new BoardState(i_Original.ships, i_Original.stars, i_NewStatus);
		}

		public static BoardState RemoveColorFromStar(BoardState i_Original, int i_StarIdx, ePipColor i_ColorToRemove)
		{
			if (i_StarIdx < 0 || i_Original.stars == null || i_StarIdx >= i_Original.stars.Count)
			{
				throw new ArgumentOutOfRangeException("Attempt to remove star with invalid index!");
			}

			List<Ship> ships = i_Original.ships
				.Where(ship => (ship.StarIdx != i_StarIdx) || (ship.Color != i_ColorToRemove))
				.ToList();
			List<IStar> stars = new List<IStar>(i_Original.stars);
			IEnumerable<ePipColor> starColors = i_Original.stars[i_StarIdx].Colors;
			if (starColors.Contains(i_ColorToRemove))
			{
				if (starColors.Count() == 1)
				{
					stars.RemoveAt(i_StarIdx);
					ships = ships.Where(ship => ship.StarIdx != i_StarIdx).ToList();
				}
				else
				{
					Homeworld oldHw = (Homeworld)i_Original.stars[i_StarIdx];
					Pip attributes = oldHw.PrimaryAttributes;
					if (oldHw.PrimaryAttributes.Color == i_ColorToRemove && oldHw.SecondaryAttributes.HasValue)
					{
						attributes = oldHw.SecondaryAttributes.Value;
					}
					stars[i_StarIdx] = new Homeworld(attributes, null, oldHw.Owner);
				}
			}

			return new BoardState(ships, stars, i_Original.status);
		}
	}
}