using System;
using System.Linq;

namespace Homeworlds.Logical
{
	[Serializable]
	public readonly struct Pip : IEquatable<Pip>
	{
		public static readonly Pip SmallRed = new Pip();
		public readonly ePipColor Color;
		public readonly ePipSize Size;
		static readonly int sr_MaxPipSize = ((ePipSize[])Enum.GetValues(typeof(ePipSize))).Max(s => (int)s);

		public Pip(ePipColor i_Color, ePipSize i_Size)
		{
			Color = i_Color;
			Size = i_Size;
		}

		public override int GetHashCode()
		{
			return sr_MaxPipSize * (int)Color + (int)Size;
		}

		public override bool Equals(object obj)
		{
			return obj is Pip && ((Pip)obj) == this;
		}

		public bool Equals(Pip other)
		{
			return other == this;
		}

		public override string ToString()
		{
			return $"{Color} {Size}";
		}

		public static bool operator ==(Pip first, Pip second)
		{
			return first.GetHashCode() == second.GetHashCode();
		}

		public static bool operator !=(Pip first, Pip second)
		{
			return !(first == second);
		}
	}
}