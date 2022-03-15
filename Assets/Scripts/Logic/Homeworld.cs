using Homeworlds.Common;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public readonly struct Homeworld : IStar
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

		public IEnumerable<ePipColor> Colors
		{
			get
			{
				List<ePipColor> result = new List<ePipColor>(2);
				result.Add(PrimaryAttributes.Color);
				if (SecondaryAttributes.HasValue)
				{
					result.Add(SecondaryAttributes.Value.Color);
				}

				return result;
			}
		}

		public IEnumerable<ePipSize> Sizes
		{
			get
			{
				List<ePipSize> result = new List<ePipSize>(2);
				result.Add(PrimaryAttributes.Size);
				if (SecondaryAttributes.HasValue)
				{
					result.Add(SecondaryAttributes.Value.Size);
				}

				return result;
			}
		}
	}
}