using System;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public readonly struct Ship : IEquatable<Ship>
	{
		public readonly Pip Attributes;
		public readonly ePlayer Owner;
		public readonly int StarIdx;

		public Ship(Pip i_Attributes, ePlayer i_Owner, int i_StarIndex)
		{
			Attributes = i_Attributes;
			Owner = i_Owner;
			StarIdx = i_StarIndex;
		}

		public ePipColor Color
		{
			get { return Attributes.Color; }
		}

		public ePipSize Size
		{
			get { return Attributes.Size; }
		}

		public bool Equals(Ship other)
		{
			return GetHashCode().Equals(other.GetHashCode());
		}

		public static bool operator ==(Ship i_First, Ship i_Second)
		{
			return i_First.Equals(i_Second);
		}

		public static bool operator !=(Ship i_First, Ship i_Second)
		{
			return !(i_First.Equals(i_Second));
		}

		public override bool Equals(object obj)
		{
			return obj is Ship other && other.Equals(this);
		}

		public override int GetHashCode()
		{
			return 24 * StarIdx + 2 * Attributes.GetHashCode() + (int)Owner;
		}

		public override string ToString()
		{
			return $"({Attributes} ship at {StarIdx}, owned by {Owner})";
		}
	}
}