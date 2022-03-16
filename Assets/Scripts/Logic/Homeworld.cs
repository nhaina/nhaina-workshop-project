﻿using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public readonly struct Homeworld : IStar, IEquatable<Homeworld>
	{
		public readonly Pip PrimaryAttributes;
		public readonly Pip? SecondaryAttributes;
		public readonly ePlayer Owner;

		public Homeworld(Pip i_PrimaryAttributes, Pip? i_SecondaryAttributes, ePlayer i_Owner)
		{
			PrimaryAttributes = i_PrimaryAttributes;
			SecondaryAttributes = i_SecondaryAttributes;
			Owner = i_Owner;
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

		public bool Equals(Homeworld other)
		{
			return GetHashCode().Equals(other.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return obj is IStar other && Equals(other);
		}

		public override int GetHashCode()
		{
			int primAtHash = PrimaryAttributes.GetHashCode();
			int secAtHash = (SecondaryAttributes?.GetHashCode()).GetValueOrDefault(-1);
			int atHash = primAtHash > secAtHash ? (secAtHash + 1) * 12 + primAtHash : (primAtHash + 1) * 12 + secAtHash;
			return atHash * 2 + (int)Owner;
		}

		public static bool operator ==(Homeworld i_First, Homeworld i_Second)
		{
			return i_First.Equals(i_Second);
		}

		public static bool operator !=(Homeworld i_First, Homeworld i_Second)
		{
			return !(i_First.Equals(i_Second));
		}

		bool IEquatable<IStar>.Equals(IStar other)
		{
			return other is Homeworld homeWorld && Equals(homeWorld);
		}
	}
}