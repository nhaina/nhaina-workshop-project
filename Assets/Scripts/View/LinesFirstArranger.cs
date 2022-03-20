using Homeworlds.Common;
using Homeworlds.Logic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Homeworlds.View
{
	public class LinesFirstArranger : IStarArranger
	{
		public float MinRowHeight { get; set; }

		public void ArrangeStars(Rect i_Bounds, IEnumerable<StarDescriptor> i_StarsToArrange)
		{
			var starGroups = i_StarsToArrange.OrderByDescending(sd => sd.Width).
				GroupBy(sd => ((Star)sd.Star).Attributes.Size);

			float rowHeight = i_Bounds.height / (2f * starGroups.Count() + 1);
			rowHeight = Mathf.Max(MinRowHeight, rowHeight);

			Rect bounds = shrinkBoundsVerticaly(i_Bounds, rowHeight, 1);

			foreach (var group in starGroups)
			{
				bounds = arrangeGroup(bounds, rowHeight, group);
			}
		}

		private Rect arrangeGroup(Rect i_Bounds, float rowHeight, IEnumerable<StarDescriptor> i_Group)
		{
			float x = i_Bounds.x;
			int row = 0;
			foreach (var item in i_Group)
			{
				x += item.Offset;
				item.gameObject.SetActive(true);
				item.transform.localPosition = new Vector3(x, 0, i_Bounds.y + row * rowHeight);
				x += item.Width;
				if (x+item.Width > i_Bounds.xMax)
				{
					x = i_Bounds.x;
					row++;
				}
			}

			return shrinkBoundsVerticaly(i_Bounds, rowHeight, row++);
		}

		private static Rect shrinkBoundsVerticaly(Rect i_Bounds, float i_RowHeight, int i_RowsToSkip)
		{
			return new Rect(i_Bounds.x, i_Bounds.y + i_RowsToSkip * i_RowHeight, i_Bounds.width, i_Bounds.height - i_RowsToSkip * i_RowHeight);
		}
	}
}