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

		public void UpdateState(BoardState i_NewState)
		{
			destroyChildren();

			List<IStar> starsAndHw = new List<IStar>(i_NewState.Stars.Cast<IStar>())
			{
				i_NewState.Player1Homeworld,
				i_NewState.Player2Homeworld
			};

			Dictionary<Pip, int> pipsOnBoard = multisetUnion(
				BoardState.toMultiset(i_NewState.Ships.Select(s => s.Attributes)),
				BoardState.toMultiset(starsAndHw.SelectMany(star => star.Attributes))
			);

			ePipColor[] colorValues = (ePipColor[])Enum.GetValues(typeof(ePipColor));
			ePipSize[] sizeValues = (ePipSize[])Enum.GetValues(typeof(ePipSize));
			foreach (ePipColor color in colorValues)
			{
				foreach (ePipSize size in sizeValues)
				{
					pipsOnBoard.TryGetValue(new Pip(color, size), out int count);
					createCell(size, color, Constants.k_MaxPipsPerColorSize - count,
						1 - (k_OffsetPercentX + (float)size) / sizeValues.Length, (k_OffsetPercentY + (float)color) / colorValues.Length);
				}
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
				y += yOffsetFromSize(i_PipSize);
			}
		}

		private float yOffsetFromSize(ePipSize i_PipSize)
		{
			float result;
			switch (i_PipSize)
			{
				case ePipSize.Large:
					result = Constants.k_VerticalSizeOffsetLarge;
					break;
				case ePipSize.Medium:
					result = Constants.k_VerticalSizeOffsetMedium;
					break;
				default:
					result = Constants.k_VerticalSizeOffsetSmall;
					break;
			}

			return result;
		}

		private static Dictionary<T, int> multisetUnion<T>(Dictionary<T, int> dictOne, Dictionary<T, int> dictTwo)
		{
			Dictionary<T, int> result = new Dictionary<T, int>();
			foreach (T key in dictOne.Keys.Union(dictTwo.Keys))
			{
				int value = 0, outValue;
				if (dictOne.TryGetValue(key, out outValue))
				{
					value += outValue;
				}
				if (dictTwo.TryGetValue(key, out outValue))
				{
					value += outValue;
				}

				result[key] = value;
			}

			return result;
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

#if UNITY_EDITOR
		private void destroyChildren()
		{
			Transform[] children = pipsContainer.Cast<Transform>().ToArray();
			foreach (Transform child in children)
			{
				DestroyImmediate(child.gameObject);
			}
		}

#else
		private void destroyChildren()
		{
			Transform[] children = pipsContainer.Cast<Transform>().ToArray();
			foreach (Transform child in children)
			{
				Destroy(child.gameObject);
			}
		}
#endif
	}
}