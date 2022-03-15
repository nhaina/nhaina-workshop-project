using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Logical;
using System;

namespace Homeworlds.View
{
	public class Bank : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] prefabs;
		[SerializeField]
		private Material[] materials;
		[SerializeField]
		private Rect bankBounds;
		[SerializeField]
		private float yOffsetPerSize;

		public GameObject[] Prefabs
		{
			get
			{
				return prefabs;
			}

			set
			{
				prefabs = value;
			}
		}

		public Rect BankBounds
		{
			get
			{
				return bankBounds;
			}

			set
			{
				bankBounds = value;
			}
		}

		public Material[] Materials
		{
			get
			{
				return materials;
			}

			set
			{
				materials = value;
			}
		}

		public Dictionary<Pip, int> State { get; set; }

		public float VerticalOffsetPerSize
		{
			get
			{
				return yOffsetPerSize;
			}

			set
			{
				yOffsetPerSize = value;
			}
		}

		public void UpdateState()
		{
#if UNITY_EDITOR
			clearChildrenEditor();
#else
			clearChildren();
#endif

			foreach (KeyValuePair<Pip, int> pair in State)
			{
				addPips(pair.Key, pair.Value);
			}
		}

		private void addPips(Pip key, int value)
		{
			Debug.Log($"{key}: {value}");
			for (int i = 0; i < value; i++)
			{
				Material mat = materials[(int)key.Color];
				GameObject pip = prefabs[(int)key.Size];
				if (pip == null || mat == null)
				{
					break;
				}
				pip = Instantiate(pip);
				pip.GetComponentInChildren<Renderer>().material = mat;
				pip.transform.parent = transform;
				pip.transform.localPosition = getPipPosition(key, i);
			}
		}

		private Vector3 getPipPosition(Pip key, int i_VerticalPlace)
		{
			float x = bankBounds.x + BankBounds.width * (int)key.Size;
			float y = i_VerticalPlace * yOffsetPerSize * (int)(key.Size + 1);
			float z = bankBounds.y + BankBounds.height * (int)key.Color;

			return new Vector3(x, y, z);

		}

		private void clearChildren()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void clearChildrenEditor()
		{
			foreach (Transform child in transform)
			{
				DestroyImmediate(child.gameObject);
			}
		}
	}
}