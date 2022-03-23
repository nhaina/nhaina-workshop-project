using Homeworlds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Homeworlds.Logic
{
	public static class Utilities
	{
		static Utilities()
		{
			s_AllPipColors = (ePipColor[])Enum.GetValues(typeof(ePipColor));
			s_AllPipSizes = (ePipSize[])Enum.GetValues(typeof(ePipSize));
			s_AllPips = new Pip[s_AllPipColors.Length * s_AllPipSizes.Length];
			int idx = 0;
			foreach (ePipColor color in s_AllPipColors)
			{
				foreach (ePipSize size in s_AllPipSizes)
				{
					s_AllPips[idx++] = new Pip(color, size);
				}
			}
		}
		private readonly static ePipColor[] s_AllPipColors;
		private readonly static ePipSize[] s_AllPipSizes;
		private readonly static Pip[] s_AllPips;

		public static IEnumerable<ePipColor> AllPipColors { get { return s_AllPipColors; } }
		public static IEnumerable<ePipSize> AllPipSizes { get { return s_AllPipSizes; } }
		public static IEnumerable<Pip> AllPips { get { return s_AllPips; } }

		public static IEnumerable<Pip> AllOf(ePipColor i_Color)
		{
			return s_AllPipSizes.Select(s => new Pip(i_Color, s));
		}

		public static IEnumerable<Pip> AllOf(ePipSize i_Size)
		{
			return s_AllPipColors.Select(c => new Pip(c, i_Size));
		}
	}
}