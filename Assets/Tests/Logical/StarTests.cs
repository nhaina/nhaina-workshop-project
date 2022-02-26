using NUnit.Framework;
using System.Collections;

namespace Homeworlds.Logical.Tests
{
	public class StarTests
	{
		[Test]
		public void StarTestEmptyPasses()
		{
			Star star = new Star();
			Assert.IsTrue(star.IsEmpty);
		}

		[Test]
		public void StarTestAddRemoveEmptyPasses()
		{
			Ship ship = new Ship();
			Star star = new Star();

			star.AddShip(ship);
			star.RemoveShip(ship);

			Assert.IsTrue(star.IsEmpty);
		}

		private static IEnumerable overpopCases()
		{
			Star star = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			yield return new object[2] { star, new ePipColor[0] };

			star = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			for (int i = 0; i < 3; i++)
			{
				star.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Red, ePipSize.Small)));
			}

			yield return new object[2] { star, new ePipColor[] {ePipColor.Red } };

			star = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			Star star2 = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			for (int i = 0; i < 4; i++)
			{
				star.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Green, ePipSize.Small)));
				star.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Red, ePipSize.Small)));
				star2.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Green, ePipSize.Small)));
			}

			yield return new object[2] { star, new ePipColor[] { ePipColor.Red, ePipColor.Green } };

			yield return new object[2] { star2, new ePipColor[] { ePipColor.Green } };
		}

		private static IEnumerable countingCases()
		{
			Star emptySmallRedStar = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			int[] emptySmallRedCounts = new int[4] { 1, 0, 0, 0 };
			yield return new object[2] { emptySmallRedStar, emptySmallRedCounts };

			Star nonemptySmallRedStar = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			nonemptySmallRedStar.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Red, ePipSize.Small)));
			nonemptySmallRedStar.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Green, ePipSize.Medium)));
			int[] nonemptySmallRedCounts = new int[4] { 2, 1, 0, 0 };
			yield return new object[2] { nonemptySmallRedStar, nonemptySmallRedCounts };

			Star nonemptySmallGreenStar = new Star(new Pip(ePipColor.Red, ePipSize.Small));
			for (int i = 0; i < 2; i++)
			{
				nonemptySmallGreenStar.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Green, ePipSize.Small)));
				nonemptySmallGreenStar.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Blue, ePipSize.Medium)));
			}

			nonemptySmallGreenStar.AddShip(new Ship(ePlayer.Player1, new Pip(ePipColor.Blue, ePipSize.Medium)));
			int[] nonemptySmallGreenCounts = new int[4] { 0, 3, 3, 0 };
			yield return new object[2] { nonemptySmallRedStar, nonemptySmallRedCounts };
		}

		[TestCaseSource(nameof(overpopCases))]
		public void StarTestOverpopulatedPasses(Star i_Star, ePipColor[] i_OverpopulatedColors)
		{
			Assert.AreEqual(i_OverpopulatedColors, i_Star.GetOverpopulatedColors());
		}

		[TestCaseSource(nameof(countingCases))]
		public void StarTestCountPasses(Star i_Star, int[] i_Counts)
		{
			int[] starCounts = new int[4];
			for (int i = 0; i < starCounts.Length; i++)
			{
				starCounts[i] = i_Star.GetColorCount((ePipColor)i);
			}
			Assert.AreEqual(i_Counts, starCounts);
		}
	}
}
