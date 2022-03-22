using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Homeworlds.View
{
	public class LinesFirstArranger : IStarArranger
	{
		public float RowHeight { get; set; }
		public float StarPadding { get; set; }

		private Vector3 nextPosition;
		private Vector3 rowStart;
		private float maxRowWidth;
		private int rowsCount;

		public void ArrangeStars(Rect i_Bounds, IEnumerable<StarDescriptor> i_StarsToArrange)
		{
			rowStart = new Vector3(i_Bounds.x, 0, i_Bounds.y);
			nextPosition = rowStart;
			arrangeByLines(i_StarsToArrange, i_Bounds.width, arrangeRow);
		}

		private void arrangeRow(IEnumerable<StarDescriptor> row)
		{
			foreach (StarDescriptor sd in row)
			{
				sd.transform.localPosition = new Vector3(sd.Offset, 0, 0) + nextPosition;
				nextPosition += new Vector3(sd.Width + sd.Offset + StarPadding, 0, 0);
			}

			nextPosition = new Vector3(rowStart.x, 0, nextPosition.z + RowHeight);
		}


		private void arrangeByLines(IEnumerable<StarDescriptor> children, float maxWidth, Action<IEnumerable<StarDescriptor>> layRowAction)
		{
			float rowWidth = 0;
			LinkedList<StarDescriptor> row = new LinkedList<StarDescriptor>();

			foreach (StarDescriptor starDescriptor in children)
			{
				float totalElementWidth = starDescriptor.Offset + starDescriptor.Width;
				if (rowWidth + totalElementWidth > maxWidth)
				{
					layRowAction(row);
					row.Clear();
					rowWidth = 0;
				}
				row.AddLast(starDescriptor);
				rowWidth += totalElementWidth;
			}
			if (row.Count > 0)
			{
				layRowAction(row);
			}
		}

		public Vector2 CalculateBounds(Rect i_Bounds, IEnumerable<StarDescriptor> i_StarsToArrange)
		{
			rowsCount = 0;
			maxRowWidth = 0;
			arrangeByLines(i_StarsToArrange, i_Bounds.width, getRowBounds);

			return 0.05f * Vector2.one + new Vector2(maxRowWidth, rowsCount * RowHeight);
		}

		private void getRowBounds(IEnumerable<StarDescriptor> row)
		{
			rowsCount++;
			maxRowWidth = Mathf.Max(maxRowWidth, row.Sum(sd => sd.Width + sd.Offset + StarPadding));
		}
	}
}