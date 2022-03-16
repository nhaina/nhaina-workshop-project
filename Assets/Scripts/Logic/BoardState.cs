using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public readonly struct BoardState : IEquatable<BoardState>
	{
		private readonly List<Ship> ships;
		private readonly List<IStar> stars;
		private readonly eBoardLifecycle status;
		private const int k_HashPrime = 13;
		private const int k_HashPrimePlusOne = k_HashPrime + 1;

		public BoardState(IEnumerable<Ship> i_Ships, IEnumerable<IStar> i_Stars, eBoardLifecycle i_Status)
		{
			ships = i_Ships.ToList();
			stars = i_Stars.ToList();
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

		public int ShipsCount
		{
			get { return ships.Count; }
		}

		public int StarsCount
		{
			get { return stars.Count; }
		}

		#region Static Factory Methods

		public static BoardState Empty;

		public static BoardState AddShip(BoardState i_Original, Ship i_ShipToAdd, out int o_ShipIdx)
		{
			if (i_ShipToAdd.StarIdx < 0 || i_ShipToAdd.StarIdx >= i_Original.stars.Count)
			{
				throw new ArgumentOutOfRangeException("Attempt to add ship to an invalid star index!");
			}

			List<Ship> ships = i_Original.ships == null ? new List<Ship>() : new List<Ship>(i_Original.ships);
			ships.Add(i_ShipToAdd);
			o_ShipIdx = ships.Count - 1;

			return new BoardState(ships, new List<IStar>(i_Original.stars), i_Original.status);
		}

		public static BoardState AddStar(BoardState i_Original, IStar i_StarToAdd, out int o_StarIdx)
		{
			List<IStar> stars = i_Original.stars == null ? new List<IStar>() : new List<IStar>(i_Original.stars);
			List<Ship> ships = i_Original.ships == null ? new List<Ship>() : new List<Ship>(i_Original.ships);
			stars.Add(i_StarToAdd);
			o_StarIdx = stars.Count - 1;

			return new BoardState(ships, stars, i_Original.status);
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
			IEnumerable<ePipColor> starColors = i_Original.stars[i_StarIdx].Attributes.Select(p=>p.Color);
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

		#endregion

		public override string ToString()
		{
			string shipsRepr = ships == null ? string.Empty : string.Concat(ships);
			string starsRepr = stars == null ? string.Empty : string.Concat(stars);
			return $"{shipsRepr} {starsRepr}";
		}

		public override bool Equals(object obj)
		{
			return obj is BoardState other && Equals(other);
		}

		public override int GetHashCode()
		{
			int shipsCount = (ships?.Count).GetValueOrDefault() % k_HashPrime;
			int starsCount = (stars?.Count).GetValueOrDefault() % k_HashPrime;
			return shipsCount + k_HashPrimePlusOne * (starsCount + k_HashPrimePlusOne * (int)status);
		}

		public static bool operator ==(BoardState first, BoardState second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(BoardState first, BoardState second)
		{
			return !(first == second);
		}

		public bool Equals(BoardState other)
		{
			bool result = other.status == status;

			if (stars != null && other.stars != null && stars.Count > 0 && other.stars.Count > 0)
			{
				result &= checkStars(other.stars);
			}

			if (ships != null && other.ships != null && ships.Count > 0 && other.ships.Count > 0)
			{
				result &= checkShipsEqual(other);
			}

			return result;
		}

		private bool checkStars(List<IStar> i_OtherStars)
		{
			bool result = stars.Count == i_OtherStars.Count;

			if (result && checkHomeWorlds(stars.Where(st => st is Homeworld), i_OtherStars.Where(st => st is Homeworld)))
			{
				Dictionary<Pip, int> myPipsMultiset = toPipMultiset(stars);
				Dictionary<Pip, int> otherPipsMultiset = toPipMultiset(i_OtherStars);
				if (myPipsMultiset.Keys.Count == otherPipsMultiset.Keys.Count)
				{
					foreach (Pip key in myPipsMultiset.Keys)
					{
						result &= otherPipsMultiset.ContainsKey(key) && myPipsMultiset[key] == otherPipsMultiset[key];
						if (!result)
						{
							break;
						}
					}
				}
			}

			return result;
		}

		private bool checkHomeWorlds(IEnumerable<IStar> i_Stars1, IEnumerable<IStar> i_Stars2)
		{
			return i_Stars1.Intersect(i_Stars2).SequenceEqual(i_Stars1);
		}

		private Dictionary<Pip, int> toPipMultiset(IEnumerable<IStar> i_Stars)
		{
			Dictionary<Pip, int> result = new Dictionary<Pip, int>();
			var groups = i_Stars.SelectMany(st => st.Attributes).GroupBy(p => p);
			foreach (var group in groups)
			{
				result.Add(group.Key, group.Count());
			}
			return result;
		}

		private bool checkShipsEqual(BoardState i_Other)
		{
			IStar[] starsCopy = stars.ToArray();
			var shipsAtts = ships.Select(s => new Tuple<Pip, ePlayer, IStar>(s.Attributes, s.Owner, starsCopy[s.StarIdx]));
			var otherShipsAtts = i_Other.ships.Select(s => new Tuple<Pip, ePlayer, IStar>(s.Attributes, s.Owner, i_Other.stars[s.StarIdx]));
			return shipsAtts.Intersect(otherShipsAtts).Count() == shipsAtts.Count();
		}

		public IEnumerable<Ship> Ships { get { return ships?.ToArray(); } }
		public IEnumerable<IStar> Stars { get { return stars?.ToArray(); } }

		public eBoardLifecycle Status
		{
			get
			{
				return status;
			}
		}
	}
}