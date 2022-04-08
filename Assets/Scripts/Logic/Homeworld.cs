using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public readonly struct Homeworld : IStar, IEquatable<Homeworld>
	{
		public readonly Pip PrimaryAttributes;
		public readonly Pip? SecondaryAttributes;
		public readonly ePlayer Owner;
		public readonly bool Destroyed;

		public static readonly Homeworld Empty;

		public interface IHomeWorldVisitor : IAbstractStarVisitor
		{
			void Visit(Homeworld homeworld);
		}

		public Homeworld(ePipColor i_PrimaryColor, ePipSize i_PrimarySize, ePipColor i_SecColor, ePipSize i_SecSize,
			ePlayer i_Owner, bool i_Destroyed)
			: this(new Pip(i_PrimaryColor, i_PrimarySize), new Pip(i_SecColor, i_SecSize), i_Owner, i_Destroyed)
		{ }

		public Homeworld(ePipColor i_PrimaryColor, ePipSize i_PrimarySize, ePlayer i_Owner, bool i_Destroyed)
			: this(new Pip(i_PrimaryColor, i_PrimarySize), null, i_Owner, i_Destroyed)
		{ }

		public Homeworld(Pip i_PrimaryAttributes, Pip? i_SecondaryAttributes, ePlayer i_Owner, bool i_Destroyed)
		{
			if (i_SecondaryAttributes.HasValue && i_SecondaryAttributes.Value.Size >= i_PrimaryAttributes.Size)
			{
				Pip tmp = i_SecondaryAttributes.Value;
				i_SecondaryAttributes = i_PrimaryAttributes;
				i_PrimaryAttributes = tmp;
			}
			PrimaryAttributes = i_PrimaryAttributes;
			SecondaryAttributes = i_SecondaryAttributes;
			Owner = i_Owner;
			Destroyed = i_Destroyed;
		}

		public IEnumerable<Pip> Attributes
		{
			get
			{
				Pip[] result;
				if (SecondaryAttributes.HasValue)
				{
					result = new Pip[] { PrimaryAttributes, SecondaryAttributes.Value };
				}
				else
				{
					result = new Pip[] { PrimaryAttributes };
				}

				return result;
			}
		}

		public int Identifier { get { return (int)Owner; } }

		public bool Equals(Homeworld other)
		{
			return PrimaryAttributes.Equals(other.PrimaryAttributes) &&
				SecondaryAttributes.Equals(other.SecondaryAttributes) &&
				Owner.Equals(other.Owner);
		}

		public override bool Equals(object obj)
		{
			return obj is Homeworld other && Equals(other);
		}

		public override int GetHashCode()
		{
			int primAtHash = PrimaryAttributes.GetHashCode();
			int secAtHash = (SecondaryAttributes?.GetHashCode()).GetValueOrDefault(-1);
			int atHash = primAtHash > secAtHash ? (secAtHash + 1) * 12 + primAtHash : (primAtHash + 1) * 12 + secAtHash;
			return atHash * 2 + (int)Owner;
		}

		public override string ToString()
		{
			string secAttributesRepr = SecondaryAttributes.HasValue ? $"-{SecondaryAttributes.Value}" : string.Empty;
			return $"{(Destroyed ? "Destroyed " : string.Empty)} {PrimaryAttributes}{secAttributesRepr} homeworld, owned by {Owner}";
		}

		public static bool operator ==(Homeworld i_First, Homeworld i_Second)
		{
			return i_First.Equals(i_Second);
		}

		public static bool operator !=(Homeworld i_First, Homeworld i_Second)
		{
			return !(i_First == i_Second);
		}

		bool IEquatable<IStar>.Equals(IStar other)
		{
			return Equals(other);
		}

		public static Homeworld MarkAsDestroyed(Homeworld i_Original)
		{
			return new Homeworld(i_Original.PrimaryAttributes, i_Original.SecondaryAttributes,
				i_Original.Owner, true);
		}

		public void Accept(IAbstractStarVisitor visitor)
		{
			if (visitor is IHomeWorldVisitor homeWorldVisitor)
			{
				homeWorldVisitor.Visit(this);
			}
			else
			{
				visitor.Visit();
			}
		}
	}
}