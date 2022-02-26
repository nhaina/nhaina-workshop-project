using System;

namespace Homeworlds.Logical
{
	public class Ship
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
			: this(ePlayer.Player1, Pip.Empty)
		{ }

		public Ship(ePlayer i_Owner, Pip i_Attributes)
		{
			Owner = i_Owner;
			Attributes = i_Attributes;

			m_Location = null;
		}
	}
}