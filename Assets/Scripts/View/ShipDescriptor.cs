using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using UnityEngine;

namespace Homeworlds.View
{
	public class ShipDescriptor : MonoBehaviour, ISelectable
	{
		public float LargeHangarOffset = 0.75f;
		public float MediumHangarOffset = 0.35f;
		public float SmallHangarOffset = 0.25f;
		[SerializeField]
		private Transform model;
		private Ship ship;

		public event Action<ISelectable> Selected;

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
			transform.localPosition = calcPosition(hangarOffset, i_Ship.Size);

			GameObject go = Instantiate(Store.FromPipSize(i_Ship.Size), model);
			go.transform.rotation = Quaternion.Euler(yaw, 0, 0);
			go.GetComponentInChildren<Renderer>().material = Store.FromPipColor(i_Ship.Color);
			i_Location.SetHangarOffset(i_Ship.Owner, hangarOffset + calcHangarOffset(i_Ship.Size));
		}

		private Vector3 calcPosition(float hangarOffset, ePipSize size)
		{
			return new Vector3(hangarOffset + 0.5f * calcHangarOffset(size), 0, 0);
		}

		private float calcHangarOffset(ePipSize size)
		{
			float sign = ship.Owner == ePlayer.Player1 ? 1 : -1;
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
			return sign * offset;
		}

		public ViewBoardPrefabStore Store { get; set; }

		// remove this
		public Ship Ship
		{
			get
			{
				return ship;
			}
		}

		public void Select()
		{
			OnSelected();
		}

		protected virtual void OnSelected()
		{
			Selected?.Invoke(this);
			Debug.Log($"Ship! {ship.Attributes}");
		}
	}
}