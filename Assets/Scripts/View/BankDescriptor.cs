using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Homeworlds.View
{
	public class BankDescriptor : MonoBehaviour, ISelectable
	{
		public const float k_OffsetPercentX = 0.375f;
		public const float k_OffsetPercentY = 0.5f;
		[SerializeField]
		private ViewBoardPrefabStore store;
		[SerializeField]
		private Transform pipsContainer;
		[SerializeField]
		private Transform floorModelContainer;
		[SerializeField]
		private Rect bounds;

		public event Action<ISelectable> Selected;

		public ViewBoardPrefabStore Store { get { return store; } set { store = value; } }
		public Transform PipsContainer { get { return pipsContainer; } set { pipsContainer = value; } }
		public Transform Floor { get { return floorModelContainer; } set { floorModelContainer = value; } }

		private void Awake()
		{
			createFloorModel();
		}

		private void createFloorModel()
		{
			Instantiate(store.BankFloorModelPrefab, floorModelContainer.transform);
			updateFloorModel();
		}

		public Rect Bounds
		{
			get
			{
				return bounds;
			}
			set
			{
				bounds = value;
				updateFloorModel();
			}
		}

		private void updateFloorModel()
		{
			floorModelContainer.transform.localPosition = new Vector3(bounds.x, 0, bounds.y);
			floorModelContainer.transform.localScale = new Vector3(bounds.width, 1, bounds.height);
		}

		private void OnDrawGizmos()
		{
			Vector3 center, size;
			size = transform.TransformDirection(new Vector3(bounds.width, 0f, bounds.height));
			center = transform.TransformPoint(new Vector3(bounds.center.x, 0, bounds.center.y));
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(center, size);
		}

		public void UpdateState(IReadOnlyDictionary<Pip, int> newState)
		{
			destroyChildren();

			foreach (KeyValuePair<Pip,int> pair in newState)
			{
				createCell(pair.Key.Size, pair.Key.Color, pair.Value,
					1 - (k_OffsetPercentX + (float)pair.Key.Size) / Utilities.PipSizesCount,
					(k_OffsetPercentY + (float)pair.Key.Color) / Utilities.PipColorsCount);

			}
		}

		private void createCell(ePipSize i_PipSize, ePipColor i_PipColor, int count, float xPercent, float yPercent)
		{
			Vector2 cell = new Vector2(bounds.x + bounds.width * xPercent, bounds.y + bounds.height * yPercent);
			float y = 0;
			for (int i = 0; i < count; i++)
			{
				GameObject pipObject = Instantiate(Store.FromPipSize(i_PipSize), pipsContainer);
				pipObject.GetComponentInChildren<Renderer>().material = Store.FromPipColor(i_PipColor);
				pipObject.transform.localPosition = new Vector3(cell.x, y, cell.y);
				y += Constants.VerticalOffsetFromSize(i_PipSize);
			}
		}

		public void Select()
		{
			OnSelected();
		}

		protected virtual void OnSelected()
		{
			Selected?.Invoke(this);
			Debug.Log($"Bank!");
		}

		private void destroyChildren()
		{
			Transform[] children = pipsContainer.Cast<Transform>().ToArray();
			foreach (Transform child in children)
			{
#if UNITY_EDITOR
				DestroyImmediate(child.gameObject);
#else
				Destroy(child.gameObject);
#endif
			}
		}
	}
}