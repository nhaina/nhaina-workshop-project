using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using UnityEngine;

namespace Homeworlds.View
{
	public class ShipDescriptor : MonoBehaviour
	{
		public float LargeHangarOffset = 0.45f;
		public float MediumHangarOffset = 0.35f;
		public float SmallHangarOffset = 0.25f;
		[SerializeField]
		private Transform model;
		private Ship ship;

		public void Initialize(Ship i_Ship, StarDescriptor i_Location)
		{
			ship = i_Ship;
			Transform hangar;
			float yaw = 90, hangarOffset = i_Location.GetHangarOffset(i_Ship.Owner);
			if (ship.Owner == ePlayer.Player1)
			{
				hangar = i_Location.Player1ShipsHangar;
			}
			else
			{
				hangar = i_Location.Player2ShipsHangar;
				yaw *= -1;
			}

			transform.parent = hangar;
			transform.position = calcPosition(hangarOffset, i_Ship.Size);

			GameObject go = Instantiate(Store.FromPipSize(i_Ship.Size), model);
			go.transform.rotation = Quaternion.Euler(yaw, 0, 0);
			go.GetComponentInChildren<Renderer>().material = Store.FromPipColor(i_Ship.Color);
			i_Location.SetHangarOffset(i_Ship.Owner, calcHangarOffset(hangarOffset, i_Ship.Size));
		}

		private Vector3 calcPosition(float hangarOffset, ePipSize size)
		{
			return new Vector3(calcHangarOffset(hangarOffset, size), 0, 0);
		}

		private float calcHangarOffset(float hangarOffset, ePipSize size)
		{
			float sign = Mathf.Sign(hangarOffset);
			float offset;
			switch (size)
			{
				case ePipSize.Medium:
					offset = MediumHangarOffset;
					break;
				case ePipSize.Large:
					offset = LargeHangarOffset;
					break;
				default:
					offset = SmallHangarOffset;
					break;
			}
			return hangarOffset + sign * offset;
		}

		public ViewBoardPrefabStore Store { get; set; }
	}
}