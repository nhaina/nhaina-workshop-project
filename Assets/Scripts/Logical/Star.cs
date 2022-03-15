using System;
using System.Collections;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public class Star : IEnumerable<KeyValuePair<Ship, int>>
	{
		private readonly Dictionary<Ship, int> r_Ships;
		private readonly int[] r_ShipColorCounters;
		public const int k_OverpopulationMinValue = 4;
		public Pip Attributes { get; set; }

		public Star()
			: this(Pip.SmallRed)
		{ }

		public Star(Pip i_Attributes)
		{
			Attributes = i_Attributes;
			r_Ships = new Dictionary<Ship, int>();
			r_ShipColorCounters = new int[Enum.GetValues(typeof(ePipColor)).Length];
		}


		public bool IsEmpty
		{
			get
			{
				return r_Ships.Count == 0;
			}
		}

		public bool IsDeleted { get; protected set; }

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
				if (!r_Ships.TryGetValue(i_Ship, out int counter))
				{
					counter = 0;
				}
				r_Ships[i_Ship] = counter + 1;
				i_Ship.Location = this;

				r_ShipColorCounters[(int)i_Ship.Attributes.Color]++;
			}
		}

		public void RemoveShip(Ship i_Ship)
		{
			if (i_Ship.Location == this)
			{
				r_Ships[i_Ship] -= 1;
				if (r_Ships[i_Ship] <= 0)
				{
					r_Ships.Remove(i_Ship);
				}

				r_ShipColorCounters[(int)i_Ship.Attributes.Color]--;
			}
		}

		public virtual int GetColorCount(ePipColor i_Color)
		{
			int colorCount = r_ShipColorCounters[(int)i_Color];
			if (i_Color == Attributes.Color)
			{
				colorCount++;
			}

			return colorCount;
		}

		public IEnumerator<KeyValuePair<Ship, int>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<Ship, int>>)r_Ships).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (r_Ships.Keys).GetEnumerator();
		}
	}
}