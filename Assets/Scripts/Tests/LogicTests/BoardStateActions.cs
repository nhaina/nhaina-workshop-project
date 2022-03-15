using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Homeworlds.Logic;
using Homeworlds.Common;

namespace Homeworlds.Tests.Logic
{
	public class BoardStateActions
	{
		[Test]
		public void AddShipPass()
		{
			List<IStar> stars = new List<IStar>()
			{
				new Star(ePipColor.Red, ePipSize.Medium)
			};
			Ship toAdd = new Ship(new Pip(ePipColor.Red, ePipSize.Medium), ePlayer.Player1, 0);
			BoardState original = new BoardState(new Ship[0], stars, eBoardLifecycle.Setup);
			BoardState result = new BoardState(new List<Ship> { toAdd }, stars, eBoardLifecycle.Setup);
			Assert.AreEqual(result, BoardState.AddShip(original, toAdd, out int index));
			Assert.That(index == 0);
		}

		[Test]
		public void AddStarPass()
		{
			Star toAdd = new Star(new Pip(ePipColor.Green, ePipSize.Large));
			BoardState original = BoardState.Empty;
			BoardState result = new BoardState(new Ship[0], new IStar[] { toAdd }, eBoardLifecycle.Setup);
			Assert.AreEqual(result, BoardState.AddStar(original, toAdd, out int index));
			Assert.That(index == 0);
		}

		[Test]
		public void RemoveShipAtPass()
		{
			List<IStar> stars = new List<IStar>()
			{
				new Star(ePipColor.Red, ePipSize.Medium)
			};
			Ship toRemove = new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player1, 0);
			BoardState original = new BoardState(new Ship[1] { toRemove }, stars, eBoardLifecycle.Setup);
			BoardState result = new BoardState(new Ship[0], stars, eBoardLifecycle.Setup);
			Assert.AreEqual(result, BoardState.RemoveShipAt(original, 0));
		}

		[Test]
		public void RemoveStarAtPass()
		{
			Star toRemove = new Star(ePipColor.Yellow, ePipSize.Small);
			Ship positioned = new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player1, 0);
			BoardState original = new BoardState(new Ship[1] { positioned }, new IStar[1] { toRemove }, eBoardLifecycle.Setup);
			BoardState result = new BoardState(new Ship[0], new IStar[0], eBoardLifecycle.Setup);
			Assert.AreEqual(result, BoardState.RemoveStarAt(original, 0));
		}

		[Test]
		public void ChangeGameStatusPass()
		{
			BoardState original = new BoardState(new Ship[0], new IStar[0], eBoardLifecycle.Setup);
			BoardState result = new BoardState(new Ship[0], new IStar[0], eBoardLifecycle.Ongoing);
			Assert.AreEqual(result, BoardState.ChangeGameStatus(original, eBoardLifecycle.Ongoing));
		}

		[TestCaseSource(nameof(removeColorFromStarCases))]
		public void RemoveColorFromStar(BoardState i_Original, int i_StarIdx, ePipColor i_ColorToRemove, BoardState i_Expected)
		{
			Assert.AreEqual(i_Expected, BoardState.RemoveColorFromStar(i_Original, i_StarIdx, i_ColorToRemove));
		}

		private static IEnumerable removeColorFromStarCases()
		{
			List<Ship> ships = new List<Ship>();
			List<IStar> stars = new List<IStar>();

			stars.Add(new Star(ePipColor.Red, ePipSize.Medium));
			ships.Add(new Ship(new Pip(ePipColor.Green, ePipSize.Medium), ePlayer.Player1, 0));
			BoardState mrStarmgShip = new BoardState(ships, stars, eBoardLifecycle.Setup);
			yield return new object[4] { mrStarmgShip, 0, ePipColor.Blue, mrStarmgShip };

			yield return new object[4] { mrStarmgShip, 0, ePipColor.Green, new BoardState(new Ship[0], stars, eBoardLifecycle.Setup) };

			yield return new object[4] { mrStarmgShip, 0, ePipColor.Red, BoardState.Empty };
		}
	}
}
