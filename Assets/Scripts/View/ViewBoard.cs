using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Linq;

namespace Homeworlds.View
{
	public class ViewBoard : MonoBehaviour
	{
		public BoardState current;
		public ViewBoardPrefabStore s_Store;
		List<StarDescriptor> stars = new List<StarDescriptor>();
		List<ShipDescriptor> ships = new List<ShipDescriptor>();

		public void UpdateField(BoardState newState)
		{
			DestroyChildren();

			current = newState;
			int? player1Idx = null;
			int? player2Idx = null;
			foreach (IStar star in newState.Stars)
			{
				GameObject go = Instantiate(s_Store.starPrefab, Vector3.zero, Quaternion.identity, transform);
				StarDescriptor starDesc = go.GetComponent<StarDescriptor>();
				starDesc.Store = s_Store;
				starDesc.Initialize(star);
				stars.Add(starDesc);
				if (star is Homeworld homeworld)
				{
					if (homeworld.Owner == ePlayer.Player1)
					{
						player1Idx = stars.Count;
					}
					else
					{
						player2Idx = stars.Count;
					}
				}
			}

			arrangeStars(player1Idx, player2Idx);

			foreach (Ship ship in newState.Ships)
			{
				GameObject go = Instantiate(s_Store.shipPrefab, Vector3.zero, Quaternion.identity, transform);
				ShipDescriptor shipDesc = go.GetComponent<ShipDescriptor>();
				shipDesc.Store = s_Store;
				shipDesc.Initialize(ship, stars[ship.StarIdx]);
				ships.Add(shipDesc);
			}
		}

		private void arrangeStars(int? player1Idx, int? player2Idx)
		{
			if (player1Idx.HasValue)
			{
				stars[player1Idx.Value].transform.position = new Vector3(0, 0, -3.5f);
			}
			if (player2Idx.HasValue)
			{
				stars[player2Idx.Value].transform.position = new Vector3(0, 0, 3.5f);
			}
			// TODO: Complete here
		}



#if UNITY_EDITOR
		private void DestroyChildren()
		{
			stars.Clear();
			ships.Clear();
			Transform[] children = transform.Cast<Transform>().ToArray();
			foreach (Transform child in children)
			{
				DestroyImmediate(child.gameObject);
			}
		}
#else
		private void DestroyChildren()
		{
			stars.Clear();
			ships.Clear();
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}
#endif
	}
}