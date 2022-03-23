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

		public GameObject UIButton { get { return uIButton; } set { uIButton = value; } }
		public GameObject RowPanel { get { return rowPanel; } set { rowPanel = value; } }

		public int ControlsCountInRow { get { return controlsCountInRow; } set { controlsCountInRow = value; } }
	}
}