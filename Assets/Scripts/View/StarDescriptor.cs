using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Homeworlds.View
{
	public class StarDescriptor : MonoBehaviour, ISelectable
	{
		[SerializeField]
		private Transform model;
		[SerializeField]
		private Transform player1ShipsHangar;
		[SerializeField]
		private Transform player2ShipsHangar;
		[SerializeField]
		private float player1HangarOffset;
		[SerializeField]
		private float player2HangarOffset;
		public const float k_HangarsPadding = 0.2f;
		public const float k_StarPadding = 0.2f;

		public event Action<ISelectable> Selected;

		public IStar Star { get; private set; }
		public ViewBoardPrefabStore Store { get; set; }

		public Transform Model
		{
			get
			{
				return model;
			}

			set
			{
				model = value;
			}
		}

		public Transform Player1ShipsHangar
		{
			get
			{
				return player1ShipsHangar;
			}

			set
			{
				player1ShipsHangar = value;
			}
		}

		public Transform Player2ShipsHangar
		{
			get
			{
				return player2ShipsHangar;
			}

			set
			{
				player2ShipsHangar = value;
			}
		}

		public float Width { get { return player1HangarOffset - player2HangarOffset + k_HangarsPadding; } }
		public float Offset { get { return 0.5f * k_HangarsPadding - player2HangarOffset; } }

		public float GetHangarOffset(ePlayer i_Owner)
		{
			return i_Owner == ePlayer.Player1 ? player1HangarOffset : player2HangarOffset;
		}

		public void SetHangarOffset(ePlayer i_Owner, float i_NewOffset)
		{
			if (i_Owner == ePlayer.Player1)
			{
				player1HangarOffset = i_NewOffset;
			}
			else
			{
				player2HangarOffset = i_NewOffset;
			}
		}

		public void Initialize(IStar i_Star)
		{
			Star = i_Star;
			createModel(i_Star);
		}

		private void createModel(IStar i_Star)
		{
			float offset = 0.0f;
			foreach (Pip pip in i_Star.Attributes.OrderByDescending(p => p.Size))
			{
				GameObject go = Instantiate(Store.FromPipSize(pip.Size), model.transform);
				go.transform.localPosition = new Vector3(0, offset, 0);
				go.GetComponentInChildren<Renderer>().material = Store.FromPipColor(pip.Color);
				offset += Constants.VerticalOffsetFromSize(pip.Size);
			}
		}

		public void Select()
		{
			OnSelected();
		}

		protected virtual void OnSelected()
		{
			Selected?.Invoke(this);
			Debug.Log($"Star! {string.Join(",", Star.Attributes)}");
		}
	}
}