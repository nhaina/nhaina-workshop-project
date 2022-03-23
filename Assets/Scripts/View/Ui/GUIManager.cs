using Homeworlds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Homeworlds.View
{
	public class GUIManager : MonoBehaviour
	{
		[SerializeField]
		private UIPrefabStore store;
		[SerializeField]
		private UIControlPanel mainPanel;

		public UIPrefabStore Store { get { return store; } set { store = value; } }
		public UIControlPanel MainPanel { get { return mainPanel; } set { mainPanel = value; } }

		public void PopSelection<T>(IEnumerable<T> i_Options, string i_Title, int i_SelectionNeeded = 1, int i_MaxControlsInRow = -1)
		{
			mainPanel.enabled = false;
			mainPanel.ClearChildren();
			mainPanel.SetTitle(i_Title);
			int controlsInRow = i_MaxControlsInRow > 0 ? i_MaxControlsInRow : Store.ControlsCountInRow;
			int optionsCount = i_Options.Count();
			do
			{
				int itemsInRow = Math.Min(controlsInRow, optionsCount);
				createRow(i_Options.Take(itemsInRow));
				optionsCount -= itemsInRow;
			} while (optionsCount > 0);

			mainPanel.enabled = true;
		}

		private void createRow<T>(IEnumerable<T> i_Items)
		{
			GameObject row = Instantiate(Store.RowPanel, mainPanel.transform);
			foreach (T item in i_Items)
			{
				GameObject selection = Instantiate(Store.UIButton, row.transform);
				selection.GetComponentInChildren<Text>().text = item.ToString();
			}
		}
	}
}