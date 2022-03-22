using Homeworlds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Homeworlds.Logic
{
	public class BoardManager
	{
		private readonly Dictionary<Pip, int> bank;
		private BoardState currentState;
		private Pip? sacrifiedPip;
		private int actionsCounter;
		private int sacrificeRecordedAt;

		public BoardState CurrentState
		{
			get
			{
				return currentState.Clone();
			}

			set
			{
				currentState = value;
			}
		}

		public BoardManager()
		{
			bank = new Dictionary<Pip, int>();
		}

		public void StartNewGame()
		{
			currentState = BoardState.Empty;
			sacrifiedPip = null;
			actionsCounter = 0;
			sacrificeRecordedAt = -1;
		}

		public void SetupRound(Homeworld player1Homeworld, Ship player1Mothership, Homeworld player2Homeworld, Ship player2Mothership)
		{
			currentState = BoardState.CreateInitial(player1Homeworld, player1Mothership, player2Homeworld, player2Mothership);
			currentState.Status = eBoardLifecycle.Ongoing;
			updateBankState();
			actionEnded(false);
		}

		public void DeclareCatastrophe(IStar i_Location, ePipColor i_CatastropheColor)
		{
			currentState = BoardState.RemoveColorFromStar(currentState, i_Location, i_CatastropheColor);
			updateBankState();
			actionEnded(false);
		}

		private void updateBankState()
		{
			var pipsGroups = currentState.Ships.Select(s => s.Attributes)
				.Concat(currentState.Stars.Select(s => s.Attributes))
				.Concat(currentState.Player1Homeworld.Attributes)
				.Concat(currentState.Player2Homeworld.Attributes)
				.GroupBy(p => p);
			bank.Clear();
			foreach (var group in pipsGroups)
			{
				bank[group.Key] = group.Count();
			}
		}

		public int SacrificeShip(Ship i_Sacrified)
		{
			if (sacrifiedPip.HasValue && (int)sacrifiedPip.Value.Size + sacrificeRecordedAt + 1 >= actionsCounter)
			{
				throw new InvalidOperationException("Cannot sacrifice two ships in a row!");
			}
			currentState = BoardState.RemoveShip(currentState, i_Sacrified);
			addPipToBank(i_Sacrified.Attributes);
			sacrifiedPip = i_Sacrified.Attributes;
			sacrificeRecordedAt = actionsCounter;
			actionEnded(false);
			return 1 + (int)sacrifiedPip.Value.Size;
		}

		public void RaidShip(Ship i_Initiator, Ship i_Target)
		{
			if (isSacrifiedColor(ePipColor.Green) && isColorAccessible(i_Initiator, ePipColor.Green) &&
				i_Initiator.Location.Equals(i_Target.Location) && i_Initiator.Owner != i_Target.Owner &&
				i_Initiator.Size > i_Target.Size)
			{
				currentState = BoardState.UpdateShip(currentState, i_Target, new Ship(i_Target.Attributes, i_Initiator.Owner, i_Initiator.Location));
				actionEnded();
			}
		}

		private void actionEnded(bool updateCounter = true)
		{
			if (updateCounter)
			{
				++actionsCounter;
			}
		}

		public void BuildShip(Ship i_Initiator, ePipColor i_NewShipColor)
		{
			if (isSacrifiedColor(ePipColor.Green) && isColorAccessible(i_Initiator, ePipColor.Green) &&
				isColoredShipAccessible(i_Initiator, i_NewShipColor))
			{
				bool succeed = false;
				foreach (ePipSize size in Enum.GetValues(typeof(ePipSize)))
				{
					Pip key = new Pip(i_NewShipColor, size);
					if (bank.ContainsKey(key))
					{
						currentState = BoardState.AddShip(currentState, new Ship(key, i_Initiator.Owner, i_Initiator.Location));
						removePipFromBank(key);
						actionEnded();
						succeed = true;
						break;
					}
				}
				if (!succeed)
				{
					throw new InvalidOperationException("No available pips of given color");
				}
			}
			else
			{
				// can't do that
				throw new InvalidOperationException($"Can't do that. No {ePipColor.Green} or {i_NewShipColor} available");
			}

		}

		public void TransformShip(Ship i_Initiator, ePipColor i_DestColor)
		{
			if (isSacrifiedColor(ePipColor.Blue) && isColorAccessible(i_Initiator, ePipColor.Blue))
			{
				Ship transformed = new Ship(new Pip(i_DestColor, i_Initiator.Size), i_Initiator.Owner, i_Initiator.Location);
				if (!bank.ContainsKey(transformed.Attributes))
				{
					throw new InvalidOperationException($"Can't do that. No {transformed.Attributes} pip available!");
				}
				currentState = BoardState.UpdateShip(currentState, i_Initiator, transformed);
				addPipToBank(i_Initiator.Attributes);
				removePipFromBank(transformed.Attributes);
				actionEnded();
			}
		}

		public void MoveShip(Ship i_Initiator, IStar i_NewLocation)
		{
			if (isSacrifiedColor(ePipColor.Yellow) && isColorAccessible(i_Initiator, ePipColor.Yellow) &&
				areSizesDifferent(i_Initiator.Location, i_NewLocation))
			{
				currentState = BoardState.UpdateShip(currentState, i_Initiator, new Ship(i_Initiator.Attributes, i_Initiator.Owner, i_NewLocation));
				actionEnded();
			}
		}

		private bool areSizesDifferent(IStar i_FirstStar, IStar i_SecondStar)
		{
			var first = i_FirstStar.Attributes.Select(p => p.Size);
			HashSet<ePipSize> second = new HashSet<ePipSize>(i_SecondStar.Attributes.Select(p => p.Size));
			ePipSize[] all = (ePipSize[])Enum.GetValues(typeof(ePipSize));
			return second.IsSubsetOf(all.Except(first));
		}

		private bool isSacrifiedColor(ePipColor i_QueriedColor)
		{
			return !sacrifiedPip.HasValue || sacrifiedPip.Value.Color == i_QueriedColor;
		}

		private bool isColorAccessible(Ship i_ActionInitiator, ePipColor i_QueriedColor)
		{
			return (sacrifiedPip.HasValue && sacrifiedPip.Value.Color == i_QueriedColor) ||
				i_ActionInitiator.Location.Attributes.Any(p => p.Color == i_QueriedColor) ||
				isColoredShipAccessible(i_ActionInitiator, i_QueriedColor);

		}

		private bool isColoredShipAccessible(Ship i_ActionInitiator, ePipColor i_QueriedColor)
		{
			return i_ActionInitiator.Color == i_QueriedColor || currentState.Ships.Any(
						s => s.Location.Equals(i_ActionInitiator.Location) &&
					   s.Owner == i_ActionInitiator.Owner && s.Color == i_QueriedColor);
		}

		private void removePipFromBank(Pip key)
		{
			bank[key] -= 1;
			if (bank[key] <= 0)
			{
				bank.Remove(key);
			}
		}

		private void addPipToBank(Pip key)
		{
			bank.TryGetValue(key, out int value);
			bank[key] = 1 + value;
		}
	}
}