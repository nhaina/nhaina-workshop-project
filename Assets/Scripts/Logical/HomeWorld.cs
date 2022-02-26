using System;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public class HomeWorld : Star
	{
		public ePlayer Owner { get; set; }
		public Pip? SecondaryAttributes { get; set; }

		public HomeWorld()
			: this(Pip.Empty, null, ePlayer.Player1)
		{ }

		public HomeWorld(Pip i_MainAttributes, Pip? i_SecondaryAttributes, ePlayer i_Owner)
			:base(i_MainAttributes)
		{
			Owner = i_Owner;
			if(i_SecondaryAttributes.HasValue)
			{
				List<ePipColor> colors = new List<ePipColor>(Colors);
				List<ePipSize> sizes = new List<ePipSize>(Sizes);
				colors.Add(i_SecondaryAttributes.Value.Color);
				sizes.Add(i_SecondaryAttributes.Value.Size);
				Colors = colors;
				Sizes = sizes;
			}
		}

	}
}