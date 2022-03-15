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
	}
}