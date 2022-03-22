using System;
using System.Collections;
using System.Collections.Generic;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public readonly struct Star : IStar, IEquatable<Star>
	{
		public interface IConcreteStarVisitor
		{
			void Visit(Star star);
		}

		private readonly int id;
		private static int s_IdGenerator = 0;
		public readonly Pip Attributes;

		public Star(Pip i_Attributes)
		{
			Attributes = i_Attributes;
			id = s_IdGenerator++;
		}

		public Star(ePipColor i_StarColor, ePipSize i_StarSize)
			: this(new Pip(i_StarColor, i_StarSize))
		{ }

		IEnumerable<Pip> IStar.Attributes
		{
			get
			{
				return new Pip[] { Attributes };
			}
		}

		public int Identifier { get { return id; } }

		bool IEquatable<IStar>.Equals(IStar other)
		{
			return Equals(other);
		}

		public bool Equals(Star other)
		{
			return Identifier.Equals(other.Identifier);
		}

		public override bool Equals(object obj)
		{
			return obj is Star other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Attributes.GetHashCode();
		}

		public static bool operator==(Star first, Star second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Star first, Star second)
		{
			return !(first == second);
		}

		public override string ToString()
		{
			return $"{Attributes} star";
		}

		public void Accept(IAbstractStarVisitor visitor)
		{
			if (visitor is IConcreteStarVisitor starVisitor)
			{
				starVisitor.Visit(this);
			}
			else
			{
				visitor.Visit();
			}
		}
	}
}