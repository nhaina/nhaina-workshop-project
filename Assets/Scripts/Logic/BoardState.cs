using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public sealed class BoardState : IEquatable<BoardState>, ICloneable
	{
		private readonly Dictionary<Pip, int> bank;
		private readonly List<Ship> ships;
		private readonly List<Star> stars;
		private Homeworld player1Homeworld;
		private Homeworld player2Homeworld;
		private eBoardLifecycle status;
		private Pip? sacrifiedPip;
		private const int k_HashPrime = 13;
		private const int k_HashPrimePlusOne = k_HashPrime + 1;

		public BoardState()
			: this(new List<Ship>(), new List<Star>(), eBoardLifecycle.Setup,
				 Homeworld.Empty, Homeworld.Empty)
		{ }

		private BoardState(BoardState other)
			: this(other.ships, other.stars, other.status, other.player1Homeworld, other.player2Homeworld)
		{
			sacrifiedPip = other.sacrifiedPip;
			ActionsCounter = other.ActionsCounter;
			SacrificeRecordedAt = other.SacrificeRecordedAt;
			ActivePlayer = other.ActivePlayer;
		}

		public BoardState(IEnumerable<Ship> i_Ships, IEnumerable<Star> i_Stars, eBoardLifecycle i_Status,
			Homeworld i_Player1Homeworld, Homeworld i_Player2Homeworld)
		{
			ships = i_Ships.ToList();
			stars = i_Stars.ToList();
			Status = i_Status;
			player1Homeworld = i_Player1Homeworld;
			player2Homeworld = i_Player2Homeworld;
			sacrifiedPip = null;
			ActionsCounter = 0;
			SacrificeRecordedAt = -1;
			bank = new Dictionary<Pip, int>();
			updateBankState();
		}

		private void updateBankState()
		{
			var pipsGroups = Ships.Select(s => s.Attributes)
				.Concat(Stars.Select(s => s.Attributes))
				.Concat(Player1Homeworld.Attributes)
				.Concat(Player2Homeworld.Attributes)
				.GroupBy(p => p);
			foreach (ePipColor color in Enum.GetValues(typeof(ePipColor)))
			{
				foreach (ePipSize size in Enum.GetValues(typeof(ePipSize)))
				{
					Bank[new Pip(color, size)] = 3;
				}
			}
			foreach (var group in pipsGroups)
			{
				Bank[group.Key] -= group.Count();
				if (Bank[group.Key] == 0)
				{
					bank.Remove(group.Key);
				}
				else if (Bank[group.Key] < 0)
				{
					throw new InvalidOperationException();
				}
			}
		}

		public BoardState Clone()
		{
			return new BoardState(this);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public int ShipsCount
		{
			get { return ships.Count; }
		}

		public ePlayer ActivePlayer { get; set; }

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

		internal void ResetBankState()
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is BoardState other && Equals(other);
		}

		public override int GetHashCode()
		{
			int shipsCount = (ships?.Count).GetValueOrDefault() % k_HashPrime;
			int starsCount = (stars?.Count).GetValueOrDefault() % k_HashPrime;
			return 2 * (shipsCount + k_HashPrimePlusOne * starsCount) + (int)ActivePlayer;
		}

		public bool Equals(BoardState other)
		{
			return other != null &&
				player1Homeworld == other.player1Homeworld &&
				player2Homeworld == other.player2Homeworld &&
				multisetEquals(stars, other.stars) &&
				multisetEquals(ships, other.ships) &&
				ActivePlayer == other.ActivePlayer;
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

		public Pip? SacrifiedPip
		{
			get
			{
				return sacrifiedPip;
			}

			set
			{
				sacrifiedPip = value;
				if (value != null)
				{
					SacrificeRecordedAt = ActionsCounter;
				}
				else
				{
					SacrificeRecordedAt = -1;
				}
			}
		}

		public int ActionsCounter { get; private set; }

		public int SacrificeRecordedAt { get; private set; }

		public Dictionary<Pip, int> Bank
		{
			get
			{
				return bank;
			}
		}

		public void AddShip(Ship i_ShipToAdd)
		{
			if (!IsKnownStar(i_ShipToAdd.Location))
			{
				throw new ArgumentOutOfRangeException("Attempt to add ship to an invalid star!");
			}

			ships.Add(i_ShipToAdd);
			removePipFromBank(i_ShipToAdd.Attributes);
		}

		public void UpdateShip(Ship i_OriginalShip, Ship i_UpdatedShip)
		{
			if (!IsKnownStar(i_OriginalShip.Location) || !IsKnownStar(i_UpdatedShip.Location))
			{
				throw new ArgumentException("Attempt to update ship from or to an invalid star!");
			}

			int shipIndex = ships.FindIndex(ship => ship.Equals(i_OriginalShip));
			ships[shipIndex] = i_UpdatedShip;
			if (i_OriginalShip.Attributes != i_UpdatedShip.Attributes)
			{
				addPipToBank(i_OriginalShip.Attributes);
				removePipFromBank(i_UpdatedShip.Attributes);
			}
		}

		public void AddStar(Star i_StarToAdd)
		{
			stars.Add(i_StarToAdd);

			removePipFromBank(i_StarToAdd.Attributes);
		}

		public void RemoveShip(Ship i_ToRemove)
		{
			if (!ships.Contains(i_ToRemove))
			{
				throw new ArgumentException("Attempt to remove an unknown ship!");
			}

			ships.Remove(i_ToRemove);
			addPipToBank(i_ToRemove.Attributes);
		}

		public void DestroyStar(IStar i_ToRemove)
		{
			GenericStarVisitor starVisitor = new GenericStarVisitor(removeStar, visitingHomeworld);
			i_ToRemove.Accept(starVisitor);
			updateBankState();

			void visitingHomeworld(Homeworld hw)
			{
				if (hw == player1Homeworld)
				{
					player1Homeworld = Homeworld.MarkAsDestroyed(hw);
				}
				else
				{
					player2Homeworld = Homeworld.MarkAsDestroyed(hw);
				}
			}
		}

		public void IncrementActionCounter()
		{
			ActionsCounter++;
		}

		public bool IsStarEmpty(IStar i_Star)
		{
			return ships.All(s => !s.Location.Equals(i_Star));
		}

		public bool IsHomeworldAbandoned(Homeworld i_Homeworld)
		{
			return ships.All(s => s.Owner != i_Homeworld.Owner || !s.Location.Equals(i_Homeworld));
		}

		private void removeStar(Star i_ToRemove)
		{
			stars.Remove(i_ToRemove);
			ships.RemoveAll(s => s.Location.Equals(i_ToRemove));
		}

		public void RemoveColorFromStar(IStar i_ToEdit, ePipColor i_ColorToRemove)
		{
			if (!IsKnownStar(i_ToEdit))
			{
				throw new ArgumentOutOfRangeException("Attempt to remove color from an unknown star!");
			}

			GenericStarVisitor visitor = new GenericStarVisitor(removeStar, visitingHomeworld);

			if (i_ToEdit.Attributes.Any(p => p.Color == i_ColorToRemove))
			{
				i_ToEdit.Accept(visitor);
			}
			else
			{
				ships.RemoveAll(ship => ship.Location.Equals(i_ToEdit) && (ship.Color == i_ColorToRemove));
			}
			updateBankState();

			void visitingHomeworld(Homeworld homeworld)
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
				}

				Homeworld oldHomeWorld;
				if (homeworld.Owner == ePlayer.Player1)
				{
					oldHomeWorld = player1Homeworld;
					player1Homeworld = homeworld;
				}
				else
				{
					oldHomeWorld = player2Homeworld;
					player2Homeworld = homeworld;
				}

				List<Ship> shipsToUpdate = ships.Where(s => s.Location.Equals(oldHomeWorld)).ToList();
				ships.RemoveAll(s => shipsToUpdate.Contains(s));
				ships.AddRange(shipsToUpdate
					.Where(s => s.Color != i_ColorToRemove)
					.Select(s => new Ship(s.Attributes, s.Owner, homeworld))
				);
			}
		}

		private void removePipFromBank(Pip key)
		{
			if (!Bank.ContainsKey(key))
			{
				throw new InvalidOperationException($"Attempt to remove nonexisting key! {key}");
			}
			Bank[key] -= 1;
			if (Bank[key] <= 0)
			{
				Bank.Remove(key);
			}
		}

		private void addPipToBank(Pip key)
		{
			Bank.TryGetValue(key, out int value);
			Bank[key] = 1 + value;
			if (Bank[key] > 3 || Bank[key] < 0)
			{
				throw new InvalidOperationException($"Invalid amounts Of pips in the bank! was {Bank[key]}");
			}
		}

		#region Static Factory Methods

		public static readonly BoardState Empty = new BoardState();

		public static BoardState CreateInitial(Homeworld player1Homeworld, Ship player1Mothership,
			Homeworld player2Homeworld, Ship player2Mothership)
		{
			if (player1Homeworld.Owner != ePlayer.Player1 || player1Homeworld.Owner != player1Mothership.Owner ||
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
		private static bool multisetEquals<T>(IEnumerable<T> i_First, IEnumerable<T> i_Second) where T : struct, IEquatable<T>
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

		public static Dictionary<T, int> toMultiset<T>(IEnumerable<T> i_Collection) where T : struct, IEquatable<T>
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