using System;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public class Star
	{
		private readonly LinkedList<Ship> r_Ships;
		private readonly int[] r_ShipColorCounters;
		public const int k_OverpopulationMinValue = 4;

		public Star()
			: this(Pip.Empty)
		{ }

		public Star(Pip i_Attributes)
		{
			Colors = new ePipColor[] { i_Attributes.Color };
			Sizes = new ePipSize[] { i_Attributes.Size };
			r_Ships = new LinkedList<Ship>();
			r_ShipColorCounters = new int[Enum.GetValues(typeof(ePipColor)).Length];
		}

		public IEnumerable<ePipColor> Colors { get; set; }
		public IEnumerable<ePipSize> Sizes { get; set; }


		public bool IsEmpty
		{
			get
			{
				return r_Ships.Count == 0;
			}
		}

		public IEnumerable<ePipColor> GetOverpopulatedColors()
		{
			List<ePipColor> overpopulated = new List<ePipColor>(4);

			for (int i = 0; i < r_ShipColorCounters.Length; i++)
			{
				if (GetColorCount((ePipColor)i) >= k_OverpopulationMinValue)
				{
					overpopulated.Add((ePipColor)i);
				}
			}

			return overpopulated;
		}

		public void AddShip(Ship i_Ship)
		{
			if (i_Ship.Location != this)
			{
				r_Ships.AddLast(i_Ship);
				i_Ship.Location = this;
				r_ShipColorCounters[(int)i_Ship.Attributes.Color]++;
			}
		}

		public void RemoveShip(Ship i_Ship)
		{
			if (i_Ship.Location == this)
			{
				r_Ships.Remove(i_Ship);
				r_ShipColorCounters[(int)i_Ship.Attributes.Color]--;
			}
		}

		public int GetColorCount(ePipColor i_Color)
		{
			int colorCount = r_ShipColorCounters[(int)i_Color];

			foreach (ePipColor color in Colors)
			{
				if (color == i_Color)
				{
					colorCount++;
				}
			}

			return colorCount;
		}
	}
}