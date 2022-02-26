namespace Homeworlds.Logical
{
	public readonly struct ResourcesState
	{
		/// two bits for each size-color tuple,
		/// where color is the leading attribute
		/// (each block of 6 bits is the same color)
		private readonly int r_State;
		private const int k_MaxCountMask = 3;
		private const int k_MaxValue = 0xffffff;

		public static ResourcesState MaxState
		{
			get
			{
				return new ResourcesState(k_MaxValue);
			}
		}

		public ResourcesState(int i_State)
		{
			r_State = i_State & k_MaxValue;
		}

		public ResourcesState(int i_SmallRedCount, int i_MediumRedCount, int i_LargeRedCount,
			int i_SmallGreenCount, int i_MediumGreenCount, int i_LargeGreenCount,
			int i_SmallBlueCount, int i_MediumBlueCount, int i_LargeBlueCount,
			int i_SmallYellowCount, int i_MediumYellowCount, int i_LargeYellowCount)
		{
			int red = i_SmallRedCount + (i_MediumRedCount << 2) + (i_LargeRedCount << 4);
			int green = i_SmallGreenCount + (i_MediumGreenCount << 2) + (i_LargeGreenCount << 4);
			int blue = i_SmallBlueCount + (i_MediumBlueCount << 2) + (i_LargeBlueCount << 4);
			int yellow = i_SmallYellowCount + (i_MediumYellowCount << 2) + (i_LargeYellowCount << 4);

			r_State = k_MaxValue & (red + (green << 6) + (blue << 12) + (yellow << 18));
		}

		private static int GetPipCountShiftAmount(ePipColor color, ePipSize size)
		{
			return 2 * (int)size + 6 * (int)color;
		}

		public int this[ePipColor color, ePipSize size]
		{
			get
			{
				int shftPos = GetPipCountShiftAmount(color, size);
				return (r_State & (k_MaxCountMask << shftPos)) >> (shftPos);
			}
		}
	}
}