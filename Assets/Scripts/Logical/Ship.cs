using System;

namespace Homeworlds.Logical
{
	public class Ship : IEquatable<Ship>
	{
		private Star m_Location;

		public Star Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				if (m_Location != value)
				{
					m_Location = value;
					m_Location.AddShip(this);
				}
			}
		}

		public Pip Attributes { get; set; }
		public ePlayer Owner { get; set; }

		public Ship()
			: this(ePlayer.Player1, Pip.SmallRed)
		{ }

		public Ship(ePlayer i_Owner, Pip i_Attributes)
		{
			Owner = i_Owner;
			Attributes = i_Attributes;

			m_Location = null;
		}

		public override int GetHashCode()
		{
			return 2*Attributes.GetHashCode()+(int)Owner;
		}

		public override bool Equals(object obj)
		{
			return obj is Ship && ((Ship)obj) == this;
		}

		public bool Equals(Ship other)
		{
			return other == this;
		}

		public static bool operator ==(Ship first, Ship second)
		{
			return first.GetHashCode() == second.GetHashCode();
		}

		public static bool operator !=(Ship first, Ship second)
		{
			return !(first == second);
		}

	}
}