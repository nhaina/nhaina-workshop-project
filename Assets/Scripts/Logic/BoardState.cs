using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public sealed class BoardState : IEquatable<BoardState>, ICloneable
	{
		private readonly List<Ship> ships;
		private readonly List<Star> stars;
		private Homeworld player1Homeworld;
		private Homeworld player2Homeworld;
		private eBoardLifecycle status;
		private const int k_HashPrime = 13;
		private const int k_HashPrimePlusOne = k_HashPrime + 1;

		public BoardState()
			: this(new List<Ship>(), new List<Star>(), eBoardLifecycle.Setup,
				 Homeworld.Empty, Homeworld.Empty)
		{ }

		public BoardState(IEnumerable<Ship> i_Ships, IEnumerable<Star> i_Stars, eBoardLifecycle i_Status,
			Homeworld i_Player1Homeworld, Homeworld i_Player2Homeworld)
		{
			ships = i_Ships.ToList();
			stars = i_Stars.ToList();
			Status = i_Status;
			player1Homeworld = i_Player1Homeworld;
			player2Homeworld = i_Player2Homeworld;
		}

		public BoardState Clone()
		{
			return new BoardState(ships, stars, Status, player1Homeworld, player2Homeworld);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public int ShipsCount
		{
			get { return ships.Count; }
		}

		public int StarsCount
		{
			get { return stars.Count; }
		}

		public bool IsKnownStar(IStar i_Star)
		{
			bool result = stars != null && (i_Star is Star star) && stars.Contains(star);
			result |= player1Homeworld.Equals(i_Star);
			result |= player2Homeworld.Equals(i_Star);
			return result;
		}

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
			return shipsCount + k_HashPrimePlusOne * starsCount;
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
			return (player1Homeworld == other.player1Homeworld &&
				player2Homeworld == other.player2Homeworld &&
				multisetEquals(stars, other.stars) &&
				multisetEquals(ships, other.ships)
			);
		}

		public IEnumerable<Ship> Ships { get { return ships; } }
		public IEnumerable<Star> Stars { get { return stars; } }

		public eBoardLifecycle Status
		{
			get { return status; }
			set { status = value; }
		}

		public Homeworld Player1Homeworld
		{
			get
			{
				return player1Homeworld;
			}
		}

		public Homeworld Player2Homeworld
		{
			get
			{
				return player2Homeworld;
			}
		}

		#region Static Factory Methods

		public static readonly BoardState Empty = new BoardState();

		public static BoardState AddShip(BoardState i_Original, Ship i_ShipToAdd)
		{
			if (!i_Original.IsKnownStar(i_ShipToAdd.Location))
			{
				throw new ArgumentOutOfRangeException("Attempt to add ship to an invalid star!");
			}

			BoardState newState = i_Original.Clone();
			newState.ships.Add(i_ShipToAdd);

			return newState;
		}

		// TODO: Add Tests
		public static BoardState UpdateShip(BoardState i_Original, Ship i_OriginalShip, Ship i_UpdatedShip)
		{
			if (!i_Original.IsKnownStar(i_OriginalShip.Location) || !i_Original.IsKnownStar(i_UpdatedShip.Location))
			{
				throw new ArgumentOutOfRangeException("Attempt to update ship from or to an invalid star!");
			}

			BoardState newState = i_Original.Clone();
			int shipIndex = newState.ships.FindIndex(ship => ship.Equals(i_OriginalShip));
			newState.ships[shipIndex] = i_UpdatedShip;

			if (!i_OriginalShip.Location.Equals(i_UpdatedShip.Location))
			{
				tryRemoveStar(i_OriginalShip.Location, newState);
			}

			return newState;
		}


		public static BoardState AddStar(BoardState i_Original, Star i_StarToAdd)
		{
			BoardState newState = i_Original.Clone();
			newState.stars.Add(i_StarToAdd);

			return newState;
		}

		/// <summary>
		/// Removes a ship from the board. if the ship is the last in the star (exluding homeworlds)
		/// the star is also removed
		/// </summary>
		/// <param name="i_Original">the original BoardState to mutate</param>
		/// <param name="i_ToRemove">the ship to remove</param>
		/// <returns></returns>
		public static BoardState RemoveShip(BoardState i_Original, Ship i_ToRemove)
		{
			if (!i_Original.ships.Contains(i_ToRemove))
			{
				throw new ArgumentOutOfRangeException("Attempt to remove an unknown ship!");
			}

			BoardState newState = i_Original.Clone();
			newState.ships.Remove(i_ToRemove);
			tryRemoveStar(i_ToRemove.Location, newState);

			return newState;
		}

		private static void tryRemoveStar(IStar i_Star, BoardState newState)
		{
			if (i_Star is Star star && newState.ships.All(s => !s.Location.Equals(i_Star)))
			{
				newState.stars.Remove(star);
			}
		}

		public static BoardState RemoveColorFromStar(BoardState i_Original, IStar i_ToEdit, ePipColor i_ColorToRemove)
		{
			if (!i_Original.IsKnownStar(i_ToEdit))
			{
				throw new ArgumentOutOfRangeException("Attempt to remove an unknown star!");
			}

			BoardState newState = i_Original.Clone();
			Predicate<Ship> shipRemovalPolicy = ship => ship.Location.Equals(i_ToEdit) && (ship.Color == i_ColorToRemove);
			GenericStarVisitor visitor = new GenericStarVisitor(visitor_VisitingStar, visitor_VisitingHomeworld);

			if (i_ToEdit.Attributes.Any(p => p.Color == i_ColorToRemove))
			{
				i_ToEdit.Accept(visitor);
			}

			newState.ships.RemoveAll(shipRemovalPolicy);
			return newState;

			void visitor_VisitingStar(Star star)
			{
				shipRemovalPolicy = ship => ship.Location.Equals(i_ToEdit);
				newState.stars.Remove(star);
			}

			void visitor_VisitingHomeworld(Homeworld homeworld)
			{
				if (homeworld.SecondaryAttributes.HasValue)
				{
					Pip primary = homeworld.PrimaryAttributes;
					if (homeworld.PrimaryAttributes.Color == i_ColorToRemove)
					{
						primary = homeworld.SecondaryAttributes.Value;
					}
					homeworld = new Homeworld(primary, null, homeworld.Owner, false);
				}
				else
				{
					homeworld = new Homeworld(homeworld.PrimaryAttributes, null, homeworld.Owner, true);
					shipRemovalPolicy = null;
				}

				if (homeworld.Owner == ePlayer.Player1)
				{
					newState.player1Homeworld = homeworld;
				}
				else
				{
					newState.player2Homeworld = homeworld;
				}
			}
		}

		public static BoardState CreateInitial(Homeworld player1Homeworld, Ship player1Mothership, 
			Homeworld player2Homeworld, Ship player2Mothership)
		{
			if (player1Homeworld.Owner != ePlayer.Player1 ||  player1Homeworld.Owner != player1Mothership.Owner ||
				!player1Mothership.Location.Equals(player1Homeworld))
			{
				throw new ArgumentException("Player1 Mothership and homeworld are not at sync!");
			}
			if (player2Homeworld.Owner != ePlayer.Player2 || player2Homeworld.Owner != player2Mothership.Owner ||
				!player2Mothership.Location.Equals(player2Homeworld))
			{
				throw new ArgumentException("Player2 Mothership and homeworld are not at sync!");
			}
			return new BoardState(new Ship[] { player1Mothership, player2Mothership }, Array.Empty<Star>(),
				eBoardLifecycle.Setup, player1Homeworld, player2Homeworld);
		}

		#endregion

		/// <summary>
		/// Converts each enumerable to a multiset, and checks for equity. returns <code>true</code> if both null or if both contain
		/// the same keys and agrees on each key value
		/// </summary>
		/// <typeparam name="T">a type the implements the IEquatable<T> interface</typeparam>
		/// <param name="i_First">the first enumerable</param>
		/// <param name="i_Second">the second enumerable</param>
		/// <returns>
		/// <code>true</code> if both null or if both contain the same keys and agrees on each key value. otherwise <code>false</code>
		/// </returns>
		private static bool multisetEquals<T>(IEnumerable<T> i_First, IEnumerable<T> i_Second) where T : IEquatable<T>
		{
			bool isFirstNull = i_First == null;
			bool isSecondNull = i_Second == null;
			bool result = isFirstNull == isSecondNull;

			if (result && !isFirstNull && i_First.Count() == i_Second.Count())
			{
				Dictionary<T, int> myPipsMultiset = toMultiset(i_First);
				Dictionary<T, int> otherPipsMultiset = toMultiset(i_Second);
				result = myPipsMultiset.Count == otherPipsMultiset.Count && !myPipsMultiset.Except(otherPipsMultiset).Any();
			}

			return result;
		}

		public static Dictionary<T, int> toMultiset<T>(IEnumerable<T> i_Collection)
		{
			Dictionary<T, int> result = new Dictionary<T, int>();
			foreach (var group in i_Collection.GroupBy(o => o))
			{
				result.Add(group.Key, group.Count());
			}
			return result;
		}
	}
}