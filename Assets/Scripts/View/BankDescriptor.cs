using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Homeworlds.View
{
	public class BankDescriptor : MonoBehaviour
	{
		public const int k_MaxPipsPerCell = 3;
		[SerializeField]
		private ViewBoardPrefabStore store;
		[SerializeField]
		private Transform pipsContainer;
		[SerializeField]
		private Rect bounds;

		public ViewBoardPrefabStore Store { get { return store; } set { store = value; } }
		public Transform PipsContainer { get { return pipsContainer; } set { pipsContainer = value; } }
		public Rect Bounds { get { return bounds; } set { bounds = value; } }

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

			float x = 0, y = 0;
			int prefabCount = store.pipPrefabs.Count, materialCount = store.materials.Count;
			foreach (GameObject prefab in store.pipPrefabs)
			{
				x = 0;
				foreach (Material material in store.materials)
				{
					if (pipsOnBoard.TryGetValue(new Pip((ePipColor)x, (ePipSize)y), out int count))
					{
						createCell(prefab, material, k_MaxPipsPerCell - count, x / (materialCount), y / (prefabCount));
					}
					x++;
				}
				y++;
			}
		}

		private void createCell(GameObject prefab, Material material, int count, float xPercent, float yPercent)
		{
			/// 0.08
			/// 0.2
			/// 0.3
			Vector2 cell = new Vector2(bounds.x + bounds.width * xPercent, bounds.y + bounds.height * yPercent);
			float y = 0;
			for (int i = 0; i < count; i++)
			{
				GameObject pipObject = Instantiate(prefab, pipsContainer);
				pipObject.GetComponentInChildren<Renderer>().material = material;
				pipObject.transform.localPosition = new Vector3(cell.x, y, cell.y);
				y += 0.2f;
			}
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

				if (value > 3)
				{
					Debug.Log($"{key}, ships: {(dictOne.ContainsKey(key)? dictOne[key] : 0)}, stars: {(dictTwo.ContainsKey(key) ? dictTwo[key] : 0)}");
				}
				result[key] = value;
			}

			return result;
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