using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Homeworlds.View
{
	public interface IStarArranger
	{
		void ArrangeStars(Rect i_Bounds, IEnumerable<StarDescriptor> i_StarsToArrange);
	}
}