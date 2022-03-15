using NUnit.Framework;
using Moq;
using System.Collections;
using System.Collections.Generic;

namespace Homeworlds.Logical.Tests
{
	public class ResourcesManagerTests
	{
		private static IEnumerable creationCases()
		{
			yield return new object[] { new Dictionary<Pip, int>(), ResourcesState.Empty };

			Dictionary<Pip, int> redOneOfEachYellowTwoLarge = new Dictionary<Pip, int>();
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Small), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Medium), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Large), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Yellow, ePipSize.Large), 2);
			yield return new object[] {redOneOfEachYellowTwoLarge, new ResourcesState(0x800015)};

			Dictionary<Pip, int> blueThreeMediumGreenOneLargeOneSmall = new Dictionary<Pip, int>();
			blueThreeMediumGreenOneLargeOneSmall.Add(new Pip(ePipColor.Blue, ePipSize.Medium), 3);
			blueThreeMediumGreenOneLargeOneSmall.Add(new Pip(ePipColor.Green, ePipSize.Large), 1);
			blueThreeMediumGreenOneLargeOneSmall.Add(new Pip(ePipColor.Green, ePipSize.Small), 1);
			yield return new object[] { blueThreeMediumGreenOneLargeOneSmall, new ResourcesState(0xc440) };
		}

		private static IEnumerable addPipPassCases()
		{
			yield return new object[] { ResourcesState.Empty, new Pip(ePipColor.Red, ePipSize.Small), new ResourcesState(0x1)};
			yield return new object[] { new ResourcesState(0xffbfff), new Pip(ePipColor.Blue, ePipSize.Medium),ResourcesState.Full};
			yield return new object[] { new ResourcesState(0xfff7ff), new Pip(ePipColor.Green, ePipSize.Large), new ResourcesState(0xfffbff) };
			yield return new object[] { new ResourcesState(0x040000), new Pip(ePipColor.Yellow, ePipSize.Small), new ResourcesState(0x080000) };
		}

		private static IEnumerable removePipPassCases()
		{
			yield return new object[] { ResourcesState.Full, new Pip(ePipColor.Red, ePipSize.Small), new ResourcesState(0xfffffe) };
			yield return new object[] { ResourcesState.Full, new Pip(ePipColor.Blue, ePipSize.Medium), new ResourcesState(0xffbfff) };
			yield return new object[] { new ResourcesState(0xfffbff), new Pip(ePipColor.Green, ePipSize.Large), new ResourcesState(0xfff7ff) };
			yield return new object[] { new ResourcesState(0x080000), new Pip(ePipColor.Yellow, ePipSize.Small), new ResourcesState(0x040000) };
		}

		private static IEnumerable addManyPipsCases()
		{
			Dictionary<Pip, int> redOneOfEachYellowTwoLarge = new Dictionary<Pip, int>();
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Small), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Medium), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Red, ePipSize.Large), 1);
			redOneOfEachYellowTwoLarge.Add(new Pip(ePipColor.Yellow, ePipSize.Large), 2);
			yield return new object[] { ResourcesState.Empty, redOneOfEachYellowTwoLarge, new ResourcesState(0x800015) };

			Dictionary<Pip, int> b3MedG2Lrg2SmlY1Sml = new Dictionary<Pip, int>();
			b3MedG2Lrg2SmlY1Sml.Add(new Pip(ePipColor.Blue, ePipSize.Medium), 3);
			b3MedG2Lrg2SmlY1Sml.Add(new Pip(ePipColor.Green, ePipSize.Large), 2);
			b3MedG2Lrg2SmlY1Sml.Add(new Pip(ePipColor.Green, ePipSize.Small), 2);
			b3MedG2Lrg2SmlY1Sml.Add(new Pip(ePipColor.Yellow, ePipSize.Small), 1);
			yield return new object[] { new ResourcesState(0x551555), b3MedG2Lrg2SmlY1Sml, new ResourcesState(0x59ddd5) };
		}

		[TestCaseSource(nameof(creationCases))]
		public void ValidateCreationPasses(Dictionary<Pip, int> i_CreationParams, ResourcesState i_Result)
		{
			Assert.AreEqual(i_Result ,new ResourcesManager().CreateResourcesState(i_CreationParams));
		}

		[Test]
		public void OutOfRangeCreationThrows()
		{
			Dictionary<Pip, int> creationDict = new Dictionary<Pip, int>();
			creationDict.Add(new Pip(), 4);
			Assert.Throws<System.ArgumentOutOfRangeException>(() => new ResourcesManager().CreateResourcesState(creationDict));
		}

		[Test]
		public void InvalidColorCreationThrows()
		{
			Dictionary<Pip, int> creationDict = new Dictionary<Pip, int>();
			creationDict.Add(new Pip((ePipColor)(-1), ePipSize.Medium), 2);
			Assert.Throws<System.ArgumentException>(() => new ResourcesManager().CreateResourcesState(creationDict));
		}

		[TestCaseSource(nameof(addPipPassCases))]
		public void AddPipPasses(ResourcesState i_StartState, Pip i_ToAdd, ResourcesState i_ExpectedResult)
		{
			ResourcesManager mgr = new ResourcesManager();

			mgr.BoardManager = createBoardManagerDummy(new BoardState(i_StartState, null, ePlayer.Player1));
			if (!mgr.TryAddPip(i_ToAdd))
			{
				Assert.Fail($"TryAdd failed!, startState: {i_StartState}, toAdd: {i_ToAdd}");
			}
			Assert.AreEqual(i_ExpectedResult, mgr.BoardManager.Current.Bank);
		}

		[TestCaseSource(nameof(removePipPassCases))]
		public void RemovePipPasses(ResourcesState i_StartState, Pip i_ToRemove, ResourcesState i_ExpectedResult)
		{
			ResourcesManager mgr = new ResourcesManager();
			mgr.BoardManager = createBoardManagerDummy(new BoardState(i_StartState, null, ePlayer.Player1));
			if (!mgr.TryRemovePip(i_ToRemove))
			{
				Assert.Fail($"TryRemove failed!, startState: {i_StartState}, toRemove: {i_ToRemove}");
			}
			Assert.AreEqual(i_ExpectedResult, mgr.BoardManager.Current.Bank);
		}

		[Test]
		public void AddInvalidPipPass()
		{
			ResourcesManager mgr = new ResourcesManager();
			BoardState boardState = new BoardState(ResourcesState.Full, null, ePlayer.Player1);

			mgr.BoardManager = createBoardManagerDummy(boardState);
			Assert.IsFalse(mgr.TryAddPip(new Pip(ePipColor.Red, ePipSize.Small)));
		}

		[TestCaseSource(nameof(addManyPipsCases))]
		public void AddManyPipPass(ResourcesState i_StartState, Dictionary<Pip, int> i_ValuesToAdd, ResourcesState i_ExpectedResult)
		{
			ResourcesManager mgr = new ResourcesManager();
			BoardState boardState = new BoardState(i_StartState, null, ePlayer.Player1);

			mgr.BoardManager = createBoardManagerDummy(boardState);

			mgr.AddPips(i_ValuesToAdd);
			Assert.AreEqual(i_ExpectedResult, mgr.BoardManager.Current.Bank);
		}

		private static IBoardManager createBoardManagerDummy(BoardState boardState)
		{
			Mock<IBoardManager> dummy = new Mock<IBoardManager>();
			dummy.SetupGet(bm => bm.Current).Returns(boardState);
			dummy.Setup(bs => bs.UpdateResources(It.IsAny<ResourcesState>()))
				.Callback<ResourcesState>(rs => dummy.SetupGet(bm => bm.Current).Returns(new BoardState(rs, null, ePlayer.Player1)));
			return dummy.Object;
		}


	}
}
