using NUnit.Framework;

namespace Homeworlds.Logical.Tests
{
	public class ResourcesStateTest
	{
		private readonly static object[] rs_ShiftCases = new object[5]
		{
			new object[4] {new ResourcesState(0,0,0,0,0,0,0,0,0,0,0,0), ePipColor.Red, ePipSize.Small, 0},
			new object[4] {new ResourcesState(1,2,3,0,3,1,2,0,0,1,3,2), ePipColor.Red, ePipSize.Large, 3 },
			new object[4] {new ResourcesState(1,2,3,0,3,1,2,0,0,1,3,2), ePipColor.Green, ePipSize.Large, 1 },
			new object[4] {new ResourcesState(1,2,3,0,3,1,2,0,0,1,3,2), ePipColor.Blue, ePipSize.Medium, 0 },
			new object[4] {new ResourcesState(1,2,3,0,3,1,2,0,0,1,3,2), ePipColor.Yellow, ePipSize.Large, 2 }
		};

		[TestCaseSource(nameof(rs_ShiftCases))]
		public void ValidateShiftPasses(ResourcesState i_ReState, ePipColor i_Color, ePipSize i_Size, int i_Result)
		{
			Assert.AreEqual(i_Result, i_ReState[i_Color, i_Size]);
		}

	}
}
