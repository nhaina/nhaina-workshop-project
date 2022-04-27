using Homeworlds.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Homeworlds.Logic
{
	public class BoardManager
	{
		private BoardState currentState;
		public event Action BoardFinishSetup;
		public event Action<BoardState> BoardUpdated;
		public event Action<eBoardLifecycle> GameEnded;
		public event Action TurnEnded;

		public BoardState CurrentState
		{
			get
			{
				return currentState;
			}

			set
			{
				currentState = value;
			}
		}

		public MovesFactory MovesFactory { get; set; }

		public IReadOnlyDictionary<Pip, int> BankState { get { return CurrentState.Bank; } }

		public BoardManager()
		{
			MovesFactory = new MovesFactory();
		}

		protected virtual void OnBoardUpdated()
		{
			BoardUpdated?.Invoke(CurrentState);
		}

		protected virtual void OnBoardFinishSetup()
		{
			BoardFinishSetup?.Invoke();
		}

		protected virtual void OnGameEnded(eBoardLifecycle i_BoardStatus)
		{
			GameEnded?.Invoke(i_BoardStatus);
		}

		public void StartNewGame()
		{
			CurrentState = BoardState.Empty;
		}

		// TODO: Add tests. Might break if eBoardLifecycle will change
		public void CheckBoardStatus()
		{
			if (CurrentState.Status == eBoardLifecycle.Ongoing)
			{
				bool hw1DestroyedOrEmpty = CurrentState.Player1Homeworld.Destroyed || CurrentState.IsHomeworldAbandoned(CurrentState.Player1Homeworld);
				bool hw2DestroyedOrEmpty = CurrentState.Player2Homeworld.Destroyed || CurrentState.IsHomeworldAbandoned(CurrentState.Player2Homeworld);
				int status = (hw1DestroyedOrEmpty ? 2 : 0) + (hw2DestroyedOrEmpty ? 1 : 0);
				CurrentState.Status = 1 + (eBoardLifecycle)status;
			}
			if (CurrentState.Status != eBoardLifecycle.Ongoing && CurrentState.Status != eBoardLifecycle.Setup)
			{
				OnGameEnded(CurrentState.Status);
			}
		}

		public void SetupGame(Homeworld player1Homeworld, Ship player1Mothership, Homeworld player2Homeworld, Ship player2Mothership)
		{
			if (CurrentState == null || CurrentState.Status != eBoardLifecycle.Setup)
			{
				throw new InvalidOperationException("Cannot setup an ended or ongoing game!");
			}

			CurrentState = BoardState.CreateInitial(player1Homeworld, player1Mothership, player2Homeworld, player2Mothership);
			CurrentState.Status = eBoardLifecycle.Ongoing;
			OnBoardFinishSetup();
			actionEnded(false);
		}

		public void DeclareCatastrophe(IStar i_Location, ePipColor i_CatastropheColor)
		{
			CurrentState.RemoveColorFromStar(i_Location, i_CatastropheColor);
			actionEnded(false);
		}

		public IEnumerable<IBoardMove> CanDeclareCatastrophe(IStar star)
		{
			List<IBoardMove> catMoves = CurrentState.Ships.Where(s => s.Location.Equals(star))
				.Select(s => s.Attributes).Concat(star.Attributes)
				.GroupBy(p => p.Color).Where(grp => grp.Count() >= 4)
				.Select(grp => MovesFactory.CreateCatastropheMove(star, grp.Key)).ToList();
			catMoves.ForEach(c => c.BoardManager = this);
			return catMoves;
		}

		public int SacrificeShip(Ship i_Sacrified)
		{
			Pip? sacrified = CurrentState.SacrifiedPip;
			if (sacrified.HasValue &&
				(int)sacrified.Value.Size + CurrentState.SacrificeRecordedAt >= CurrentState.ActionsCounter)
			{
				throw new InvalidOperationException("Cannot sacrifice two ships in a row!");
			}

			CurrentState.RemoveShip(i_Sacrified);
			tryRemoveStar(i_Sacrified.Location);
			CurrentState.SacrifiedPip = i_Sacrified.Attributes;
			actionEnded(false);
			return 1 + (int)i_Sacrified.Attributes.Size;
		}

		private bool tryRemoveStar(IStar i_ToRemove)
		{
			bool removed = false;
			if (i_ToRemove is Star toRemoveAsStar && CurrentState.IsStarEmpty(i_ToRemove))
			{
				CurrentState.DestroyStar(toRemoveAsStar);
				removed = true;
			}

			return removed;
		}

		public void RaidShip(Ship i_Initiator, Ship i_Target)
		{
			if (isSacrifiedColor(ePipColor.Red) && isColorAccessible(i_Initiator, ePipColor.Red) &&
				i_Initiator.Location.Equals(i_Target.Location) && i_Initiator.Owner != i_Target.Owner &&
				i_Initiator.Size >= i_Target.Size)
			{
				CurrentState.UpdateShip(i_Target, new Ship(i_Target.Attributes, i_Initiator.Owner, i_Initiator.Location));
				actionEnded();
			}
		}

		public ePlayer ActivePlayer
		{
			get
			{
				return CurrentState.ActivePlayer;
			}
			set
			{
				CurrentState.ActivePlayer = value;
			}
		}

		public eBoardLifecycle BoardStatus
		{
			get { return CurrentState.Status; }
		}

		public void EndTurn()
		{
			actionEnded(true, true);
		}

		private void actionEnded(bool updateCounter = true, bool forceEnd = false)
		{
			if (updateCounter)
			{
				CurrentState.IncrementActionCounter();
				if (CurrentState.SacrifiedPip.HasValue)
				{
					if (forceEnd || CurrentState.ActionsCounter > (int)CurrentState.SacrifiedPip.Value.Size + CurrentState.SacrificeRecordedAt)
					{
						CurrentState.SacrifiedPip = null;
						turnEnded();
					}
				}
				else
				{
					turnEnded();
				}
			}

			OnBoardUpdated();
		}

		private void turnEnded()
		{
			CheckBoardStatus();
			CurrentState.ActivePlayer = 1 - CurrentState.ActivePlayer;
			OnTurnEnded();
		}

		protected virtual void OnTurnEnded()
		{
			TurnEnded?.Invoke();
		}

		public void BuildShip(Ship i_Initiator)
		{
			if (isSacrifiedColor(ePipColor.Green) && isColorAccessible(i_Initiator, ePipColor.Green))
			{
				bool succeed = false;
				foreach (ePipSize size in Utilities.AllPipSizes)
				{
					Pip key = new Pip(i_Initiator.Color, size);
					if (CurrentState.Bank.ContainsKey(key))
					{
						CurrentState.AddShip(new Ship(key, i_Initiator.Owner, i_Initiator.Location));
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
				throw new InvalidOperationException($"Can't do that. No {ePipColor.Green} available");
			}
		}

		public void TransformShip(Ship i_Initiator, ePipColor i_DestColor)
		{
			if (isSacrifiedColor(ePipColor.Blue) && isColorAccessible(i_Initiator, ePipColor.Blue))
			{
				Ship transformed = new Ship(new Pip(i_DestColor, i_Initiator.Size), i_Initiator.Owner, i_Initiator.Location);
				if (!CurrentState.Bank.ContainsKey(transformed.Attributes))
				{
					throw new InvalidOperationException($"Can't do that. No {transformed.Attributes} pip available!");
				}
				CurrentState.UpdateShip(i_Initiator, transformed);
				actionEnded();
			}
		}

		public void MoveShip(Ship i_Initiator, IStar i_NewLocation)
		{
			if (isSacrifiedColor(ePipColor.Yellow) && isColorAccessible(i_Initiator, ePipColor.Yellow) &&
				areSizesConsecutive(i_Initiator.Location, i_NewLocation))
			{
				if (!CurrentState.IsKnownStar(i_NewLocation) && i_NewLocation is Star newStar)
				{
					if (!CurrentState.Bank.ContainsKey(newStar.Attributes))
					{
						throw new InvalidOperationException($"Can't do that. No {newStar.Attributes} pip available!");
					}
					CurrentState.AddStar(newStar);
				}
				IStar oldLocation = i_Initiator.Location;
				CurrentState.UpdateShip(i_Initiator, new Ship(i_Initiator.Attributes, i_Initiator.Owner, i_NewLocation));
				tryRemoveStar(oldLocation);
				actionEnded();
			}
		}

		public bool IsKnownStarOrHomeworld(IStar i_Location)
		{
			return CurrentState.IsKnownStar(i_Location);
		}

		//TODO: refactor this. please
		public IEnumerable<IBoardMove> GetAvailableMoves(Ship i_Initiator)
		{
			List<IBoardMove> available = new List<IBoardMove>();
			IEnumerable<ePipColor> availableColors;
			IEnumerable<Pip> bankPips = CurrentState.Bank.Keys;
			if (!CurrentState.SacrifiedPip.HasValue)
			{
				available.Add(MovesFactory.CreateSacrificeMove(i_Initiator));
				IEnumerable<Ship> shipsNearInitiator = CurrentState.Ships.Where(ship =>
					ship.Owner == i_Initiator.Owner &&
					ship.Location.Equals(i_Initiator.Location));
				availableColors = getAvailableColors(shipsNearInitiator, i_Initiator.Location);
			}
			else
			{
				availableColors = new List<ePipColor> { CurrentState.SacrifiedPip.Value.Color };
			}

			foreach (ePipColor color in availableColors)
			{
				switch (color)
				{
					case ePipColor.Red:
						IEnumerable<Ship> enemyShips = CurrentState.Ships.Where(ship =>
						ship.Owner != i_Initiator.Owner && ship.Location.Equals(i_Initiator.Location)
						);
						available.AddRange(enemyShips.Where(es => es.Size <= i_Initiator.Size)
							.Select(es => MovesFactory.CreateRaidMove(i_Initiator, es)));
						break;
					case ePipColor.Green:
						if (bankPips.Any(p => p.Color == i_Initiator.Color))
						{
							available.Add(MovesFactory.CreateBuildMove(i_Initiator));
						}
						break;
					case ePipColor.Blue:
						available.AddRange(
							bankPips.Where(p => i_Initiator.Size == p.Size && i_Initiator.Color != p.Color)
							.Select(p => MovesFactory.CreateTransformMove(i_Initiator, p.Color)));
						break;
					case ePipColor.Yellow:
						IEnumerable<IStar> allLocations = CurrentState.Stars.Cast<IStar>()
							.Concat(new IStar[2] { CurrentState.Player1Homeworld, CurrentState.Player2Homeworld })
							.Concat(bankPips.Select(p => (IStar)new Star(p)))
							.Where(s => areSizesConsecutive(s, i_Initiator.Location));
						available.AddRange(allLocations.Select(l => MovesFactory.CreateFlyMove(i_Initiator, l)));
						break;
				}
			}

			available.ForEach(bm => bm.BoardManager = this);
			return available;
		}

		public IEnumerable<IBoardMove> GetAllAvailableMoves(ePlayer forPlayer)
		{
			return CurrentState.Ships.Where(ship => ship.Owner == forPlayer).SelectMany(ship => GetAvailableMoves(ship))
				.Concat(CurrentState.Stars.SelectMany(st => CanDeclareCatastrophe(st)))
				.Concat(CanDeclareCatastrophe(CurrentState.Player1Homeworld))
				.Concat(CanDeclareCatastrophe(CurrentState.Player2Homeworld)).ToList();
		}

		private IEnumerable<ePipColor> getAvailableColors(IEnumerable<Ship> i_Ships, IStar i_Location)
		{
			List<ePipColor> colors = i_Ships.Select(s => s.Color).Concat(i_Location.Attributes.Select(p => p.Color)).Distinct().ToList();
			if (CurrentState.SacrifiedPip.HasValue)
			{
				colors.Add(CurrentState.SacrifiedPip.Value.Color);
			}
			return colors;
		}

		private bool areSizesConsecutive(IStar i_FirstStar, IStar i_SecondStar)
		{
			var first = i_FirstStar.Attributes.Select(p => p.Size);
			HashSet<ePipSize> second = new HashSet<ePipSize>(i_SecondStar.Attributes.Select(p => p.Size));
			ePipSize[] all = (ePipSize[])Enum.GetValues(typeof(ePipSize));
			return second.IsSubsetOf(all.Except(first));
		}

		private bool isSacrifiedColor(ePipColor i_QueriedColor)
		{
			return !CurrentState.SacrifiedPip.HasValue || CurrentState.SacrifiedPip.Value.Color == i_QueriedColor;
		}

		private bool isColorAccessible(Ship i_ActionInitiator, ePipColor i_QueriedColor)
		{
			return (CurrentState.SacrifiedPip.HasValue && CurrentState.SacrifiedPip.Value.Color == i_QueriedColor) ||
				i_ActionInitiator.Location.Attributes.Any(p => p.Color == i_QueriedColor) ||
				isColoredShipAccessible(i_ActionInitiator, i_QueriedColor);

		}

		private bool isColoredShipAccessible(Ship i_ActionInitiator, ePipColor i_QueriedColor)
		{
			return i_ActionInitiator.Color == i_QueriedColor || CurrentState.Ships.Any(
						s => s.Location.Equals(i_ActionInitiator.Location) &&
					   s.Owner == i_ActionInitiator.Owner && s.Color == i_QueriedColor);
		}
	}
}