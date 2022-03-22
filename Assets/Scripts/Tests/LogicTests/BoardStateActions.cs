using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Homeworlds.Logic;
using Homeworlds.Common;
using System.Linq;

namespace Homeworlds.Tests.Logic
{
	public class BoardStateActions
	{
		[Test]
		public void AddShipPass()
		{
			Star[] stars = new Star[] { new Star(ePipColor.Red, ePipSize.Medium) };
			Ship toAdd = new Ship(new Pip(ePipColor.Red, ePipSize.Medium), ePlayer.Player1, stars[0]);
			BoardState original = new BoardState(new Ship[0], stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			BoardState result = new BoardState(new List<Ship> { toAdd }, stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			Assert.AreEqual(result, BoardState.AddShip(original, toAdd));
		}

		[Test]
		public void AddStarPass()
		{
			Star toAdd = new Star(new Pip(ePipColor.Green, ePipSize.Large));
			BoardState original = BoardState.Empty;
			BoardState result = new BoardState(new Ship[0], new Star[] { toAdd }, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			Assert.AreEqual(result, BoardState.AddStar(original, toAdd));
		}

		[Test]
		public void RemoveLastShipPass()
		{
			Star[] stars = new Star[] { new Star(ePipColor.Red, ePipSize.Medium) };
			Ship toRemove = new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player1, stars[0]);
			BoardState original = new BoardState(new Ship[1] { toRemove }, stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			BoardState result = new BoardState(new Ship[0], stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			Assert.AreEqual(result, BoardState.RemoveShip(original, toRemove));
		}

		[Test]
		public void RemoveNotLastShipPass()
		{
			Star[] stars = new Star[] { new Star(ePipColor.Red, ePipSize.Medium) };
			Ship toRemove = new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player1, stars[0]);
			Ship toKeep = new Ship(new Pip(ePipColor.Yellow, ePipSize.Small), ePlayer.Player1, stars[0]);
			BoardState original = new BoardState(new Ship[2] { toRemove, toKeep }, stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			BoardState result = new BoardState(new Ship[1] { toKeep }, stars, eBoardLifecycle.Setup, Homeworld.Empty, Homeworld.Empty);
			Assert.AreEqual(result, BoardState.RemoveShip(original, toRemove));
		}

		[TestCaseSource(nameof(removeColorFromStarCases))]
		public void RemoveColorFromStar(BoardState i_Original, IStar i_ToEdit, ePipColor i_ColorToRemove, BoardState i_Expected)
		{
			Assert.AreEqual(i_Expected, BoardState.RemoveColorFromStar(i_Original, i_ToEdit, i_ColorToRemove));
		}

		private static IEnumerable removeColorFromStarCases()
		{
			Homeworld p1 = new Homeworld(ePipColor.Green, ePipSize.Medium, ePlayer.Player1, false);
			Homeworld p2 = new Homeworld(ePipColor.Red, ePipSize.Large, ePipColor.Blue, ePipSize.Small, ePlayer.Player2, false);
			Star medRed = new Star(ePipColor.Red, ePipSize.Medium);
			List<Star> stars = new List<Star>() { medRed };
			List<Ship> ships = new List<Ship>()
			{
				new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player1, p1),
				new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player2, p1),
				new Ship(new Pip(ePipColor.Red, ePipSize.Small), ePlayer.Player2, stars[0]),
				new Ship(new Pip(ePipColor.Yellow, ePipSize.Medium), ePlayer.Player1, stars[0])
			};

			BoardState state = new BoardState(ships, stars, eBoardLifecycle.Setup, p1, p2);
			yield return new object[4] { state, medRed, ePipColor.Blue, state};

			List<Ship> shipCopy = ships.ToList();
			shipCopy.RemoveAt(3);
			yield return new object[4] { state, medRed, ePipColor.Yellow, new BoardState(shipCopy, stars, eBoardLifecycle.Setup, p1, p2) };

			shipCopy = ships.ToList();
			shipCopy.RemoveAll(s => s.Location.Equals(medRed));
			yield return new object[4] { state, medRed, ePipColor.Red, new BoardState(shipCopy, new Star[0], eBoardLifecycle.Setup, p1, p2)};

			Homeworld newP2 = new Homeworld(ePipColor.Blue, ePipSize.Small, ePlayer.Player2, false);
			yield return new object[4] { state, p2, ePipColor.Red, new BoardState(ships, stars, eBoardLifecycle.Setup, p1, newP2) };

			Homeworld newP1 = new Homeworld(ePipColor.Green, ePipSize.Medium, ePlayer.Player1, true);
			yield return new object[4] { state, p1, ePipColor.Red, new BoardState(ships, stars, eBoardLifecycle.Setup, newP1, p2) };
		}
	}
}
