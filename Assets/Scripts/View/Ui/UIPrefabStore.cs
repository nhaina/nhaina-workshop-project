using Homeworlds.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class UIPrefabStore : MonoBehaviour
	{
		[SerializeField]
		private GameObject uIButton;
		[SerializeField]
		private GameObject rowPanel;
		[SerializeField]
		private int controlsCountInRow;

		public GameObject UIButton { get { if (uIButton == null) { Debug.Log("Store->UIButton is Null!"); } return uIButton; } set { uIButton = value; } }
		public GameObject RowPanel { get { if (uIButton == null) { Debug.Log("Store->RowPanel is Null!"); }; return rowPanel; } set { rowPanel = value; } }

		public int ControlsCountInRow { get { return controlsCountInRow; } set { controlsCountInRow = value; } }
	}
}