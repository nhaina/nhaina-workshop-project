namespace Homeworlds.Logical
{
	public readonly struct ResourcesState
	{
		/// two bits for each size-color tuple,
		/// where size is the leading attribute
		/// (each block of 6 bits is the same color)
		private readonly int r_State;
		private const int k_MaxValue = 0xffffff;

		public static ResourcesState Full
		{
			get
			{
				return new ResourcesState(k_MaxValue);
			}
		}

		public static ResourcesState Empty
		{
			get
			{
				return new ResourcesState(0);
			}
		}

		public ResourcesState(int i_State)
		{
			r_State = i_State & k_MaxValue;
		}

		public static explicit operator int(ResourcesState state)
		{
			return state.r_State;
		}

		public override int GetHashCode()
		{
			return r_State.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is ResourcesState && r_State.Equals(((ResourcesState)obj).r_State);
		}

		public override string ToString()
		{
			return r_State.ToString();
		}
	}
}