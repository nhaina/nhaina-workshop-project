using System;
using System.Collections;
using System.Collections.Generic;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public readonly struct Star : IStar
	{
		public readonly Pip Attributes;

		public Star(Pip i_Attributes)
		{
			Attributes = i_Attributes;
		}

		public Star(ePipColor i_StarColor, ePipSize i_StarSize)
			: this(new Pip(i_StarColor, i_StarSize))
		{ }

		public IEnumerable<ePipColor> Colors
		{
			get
			{
				return new ePipColor[] { Attributes.Color };
			}
		}

		public IEnumerable<ePipSize> Sizes
		{
			get
			{
				return new ePipSize[] { Attributes.Size };
			}
		}

		bool IEquatable<IStar>.Equals(IStar other)
		{
			return Equals(other);
		}

		public override bool Equals(object obj)
		{
			return obj is Star other && Attributes.Equals(other.Attributes);
		}

		public override int GetHashCode()
		{
			return Attributes.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Attributes} star";
		}
	}
}