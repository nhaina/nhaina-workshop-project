using Homeworlds.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	internal class Constants : MonoBehaviour
	{
		public const int k_MaxPipsPerColorSize = 3;
		public const float k_VerticalSizeOffsetSmall = 0.08f;
		public const float k_VerticalSizeOffsetMedium = 0.2f;
		public const float k_VerticalSizeOffsetLarge = 0.3f;

		public static float VerticalOffsetFromSize(ePipSize size)
		{
			float y = 0;
			switch (size)
			{
				case ePipSize.Small:
					y = k_VerticalSizeOffsetSmall;
					break;
				case ePipSize.Medium:
					y = k_VerticalSizeOffsetMedium;
					break;
				case ePipSize.Large:
					y = k_VerticalSizeOffsetLarge;
					break;
			}
			return y;
		}
	}
}