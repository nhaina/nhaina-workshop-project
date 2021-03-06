using System;

namespace Homeworlds.Common
{
	[Serializable]
	public readonly struct Pip : IEquatable<Pip>
	{
		public readonly ePipColor Color;
		public readonly ePipSize Size;

		public Pip(ePipColor i_Color, ePipSize i_Size)
		{
			Color = i_Color;
			Size = i_Size;
		}

		public override bool Equals(object obj)
		{
			return obj is Pip other && other == this;
		}

		public override int GetHashCode()
		{
			return (int)Color + 4*(int)Size;
		}

		public override string ToString()
		{
			return $"{Size} {Color}";
		}

		public bool Equals(Pip other)
		{
			return Color == other.Color && Size == other.Size;
		}

		public static bool operator ==(Pip first, Pip second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Pip first, Pip second)
		{
			return !(first == second);
		}
	}
}
