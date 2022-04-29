using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Homeworlds.View
{
	[RequireComponent(typeof(Canvas))]
	public class GUIManager : MonoBehaviour
	{
		[SerializeField]
		private UIPrefabStore store;
		[SerializeField]
		private UIControlPanel mainPanel;
		public Action<IUIDrawable> SelectionCallback { get; set; }

		public UIPrefabStore Store { get { return store; } set { store = value; } }
		public UIControlPanel MainPanel { get { return mainPanel; } set { mainPanel = value; } }

		public bool IsUIOpen { get { return mainPanel.gameObject.activeInHierarchy; } }

		private void Awake()
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}

		public void PopSelection(IEnumerable<IUIDrawable> i_Options, string i_Title, int i_MaxControlsInRow = -1)
		{
			mainPanel.gameObject.SetActive(false);
			mainPanel.enabled = false;
			mainPanel.ClearChildren();
			mainPanel.SetTitle(i_Title);
			int controlsInRow = i_MaxControlsInRow > 0 ? i_MaxControlsInRow : Store.ControlsCountInRow;
			int optionsCount = i_Options.Count(), currentCount = optionsCount;
			do
			{
				int itemsInRow = Math.Min(controlsInRow, optionsCount);
				createRow(i_Options.Skip(optionsCount - currentCount).Take(itemsInRow));
				currentCount -= itemsInRow;
			} while (currentCount > 0);

			mainPanel.enabled = true;
			mainPanel.gameObject.SetActive(true);
		}

		private void createRow(IEnumerable<IUIDrawable> i_Drawables)
		{
			GameObject row = Instantiate(Store.RowPanel);
			mainPanel.AddChild(row);
			foreach (IUIDrawable drawable in i_Drawables)
			{
				GameObject selection = Instantiate(Store.UIButton, row.transform);
				Button buttonComp = selection.GetComponentInChildren<Button>();
				buttonComp.onClick.AddListener(() => SelectionCallback(drawable));
				selection.GetComponentInChildren<Text>().text = drawable.Content;
			}
		}

		public void CloseUI()
		{
			mainPanel.gameObject.SetActive(false);
		}

		public void PopInformation(ISelectable i_Selectable)
		{
			Debug.Log(i_Selectable);
		}
	}
}