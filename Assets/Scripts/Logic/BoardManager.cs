﻿using Homeworlds.Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
				currentState = value.Clone();
			}
		}

		public MovesFactory MovesFactory { get; set; }

		public Dictionary<Pip, int> BankState { get { return bank; } }

		public BoardManager()
		{
			bank = new Dictionary<Pip, int>();
			MovesFactory = new MovesFactory();
		}

		public void StartNewGame()
		{
			currentState = BoardState.Empty;
			sacrifiedPip = null;
			actionsCounter = 0;
			sacrificeRecordedAt = -1;
		}

		// TODO: Add tests. Might break if eBoardLifecycle will change
		public void CheckBoardStatus()
		{
			if (currentState.Status == eBoardLifecycle.Ongoing)
			{
				bool hw1DestroyedOrEmpty = currentState.Player1Homeworld.Destroyed || isEmpty(currentState.Player1Homeworld);
				bool hw2DestroyedOrEmpty = currentState.Player2Homeworld.Destroyed || isEmpty(currentState.Player2Homeworld);
				int status = (hw1DestroyedOrEmpty ? 2 : 0) + (hw2DestroyedOrEmpty ? 1 : 0);
				currentState.Status = 1 + (eBoardLifecycle)status;
			}
		}

		private bool isEmpty(Homeworld i_QueriedHomeWorld)
		{
			return currentState.Ships.All(s => !s.Location.Equals(i_QueriedHomeWorld));
		}

		public void SetupRound(Homeworld player1Homeworld, Ship player1Mothership, Homeworld player2Homeworld, Ship player2Mothership)
		{
			if (currentState.Status != eBoardLifecycle.Setup)
			{
				throw new InvalidOperationException("Cannot setup an ended or ongoing game!");
			}

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

		public IEnumerable<IBoardMove> CanDeclareCatastrophe(IStar star)
		{
			List<IBoardMove> catMoves = currentState.Ships.Where(s=>s.Location.Equals(star))
				.Select(s=>s.Attributes).Concat(star.Attributes)
				.GroupBy(p => p.Color).Where(grp => grp.Count() >= 4)
				.Select(grp => MovesFactory.CreateCatastropheMove(star, grp.Key)).ToList();
			catMoves.ForEach(c => c.BoardManager = this);
			return catMoves;
		}

		private void updateBankState()
		{
			var pipsGroups = currentState.Ships.Select(s => s.Attributes)
				.Concat(currentState.Stars.Select(s => s.Attributes))
				.Concat(currentState.Player1Homeworld.Attributes)
				.Concat(currentState.Player2Homeworld.Attributes)
				.GroupBy(p => p);
			foreach (ePipColor color in Enum.GetValues(typeof(ePipColor)))
			{
				foreach (ePipSize size in Enum.GetValues(typeof(ePipSize)))
				{
					bank[new Pip(color, size)] = 3;
				}
			}
			foreach (var group in pipsGroups)
			{
				bank[group.Key] -= group.Count();
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

		public void BuildShip(Ship i_Initiator)
		{
			if (isSacrifiedColor(ePipColor.Green) && isColorAccessible(i_Initiator, ePipColor.Green))
			{
				bool succeed = false;
				foreach (ePipSize size in Enum.GetValues(typeof(ePipSize)))
				{
					Pip key = new Pip(i_Initiator.Color, size);
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
				throw new InvalidOperationException($"Can't do that. No {ePipColor.Green} available");
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
				areDifferentSizes(i_Initiator.Location, i_NewLocation))
			{
				if (!currentState.IsKnownStar(i_NewLocation) && i_NewLocation is Star newStar)
				{
					currentState = BoardState.AddStar(currentState, newStar);
				}
				currentState = BoardState.UpdateShip(currentState, i_Initiator, new Ship(i_Initiator.Attributes, i_Initiator.Owner, i_NewLocation));
				actionEnded();
			}
		}

		//TODO: refactor this. please
		public IEnumerable<IBoardMove> GetAvailableMoves(Ship i_Initiator)
		{
			List<IBoardMove> available = new List<IBoardMove>();
			IEnumerable<Ship> shipsNearInitiator = currentState.Ships.Where(ship =>
				ship.Owner == i_Initiator.Owner &&
				ship.Location.Equals(i_Initiator.Location)
			);
			foreach (ePipColor color in getAvailableColors(shipsNearInitiator, i_Initiator.Location))
			{
				switch (color)
				{
					case ePipColor.Red:
						IEnumerable<Ship> enemyShips = currentState.Ships.Where(ship =>
						ship.Owner != i_Initiator.Owner && ship.Location.Equals(i_Initiator.Location)
						);
						available.AddRange(shipsNearInitiator.Zip(enemyShips, (s, t) => (RaidMove)MovesFactory.CreateRaidMove(s, t)).Where(rm => rm.RaiderShip.Size >= rm.TargetedShip.Size));
						break;
					case ePipColor.Green:
						available.AddRange(shipsNearInitiator.Select(ship => MovesFactory.CreateBuildMove(ship)));
						break;
					case ePipColor.Blue:
						available.AddRange(shipsNearInitiator.Join(bank.Keys, s=>s.Attributes, p=>p, (s, p) => (TransformMove)MovesFactory.CreateTransformMove(s, p.Color))
							.Where(tm => tm.DestinationColor != tm.TargetShip.Color));
						break;
					case ePipColor.Yellow:
						IEnumerable<IStar> allLocations = currentState.Stars.Cast<IStar>()
							.Concat(new IStar[2] { currentState.Player1Homeworld, currentState.Player2Homeworld })
							.Concat(bank.Keys.Select(p => (IStar)new Star(p)))
							.Where(s => areDifferentSizes(s, i_Initiator.Location));
						available.AddRange(shipsNearInitiator.Zip(allLocations, (s, l) => (FlyMove)MovesFactory.CreateFlyMove(s, l)));
						break;
				}
			}

			available.ForEach(bm => bm.BoardManager = this);

			return available;
		}

		private IEnumerable<ePipColor> getAvailableColors(IEnumerable<Ship> i_Ships, IStar i_Location)
		{
			return i_Ships.Select(s => s.Color).Concat(i_Location.Attributes.Select(p => p.Color)).Distinct();
		}

		private bool areDifferentSizes(IStar i_FirstStar, IStar i_SecondStar)
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