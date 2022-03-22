using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Homeworlds.View
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private Camera playerCamera;
		public Camera PlayerCamera { get { return playerCamera; } set { playerCamera = value; } }
		public event Action<Vector2, CancellationRequest> ClickedOutsideUI;

		private void Awake()
		{
			playerCamera = playerCamera ? playerCamera : Camera.main;

		}

		private void Update()
		{
			if (Input.touchSupported)
			{
				TestTouch();
			}
			else
			{
				TestMouse();
			}
			if (Input.GetButton("Cancel"))
			{
				Application.Quit();
			}
		}

		private void TestMouse()
		{
			if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
			{
				CastTouchRay(Input.mousePosition, 0.5f);
			}
		}

		protected bool OnClickedOutsideUI(Vector2 i_ScreenPositionClicked)
		{
			CancellationRequest request = new CancellationRequest();
			ClickedOutsideUI?.Invoke(i_ScreenPositionClicked, request);
			return request.RequestCancel;
		}

		private void TestTouch()
		{
			int touchesCount = Input.touchCount;
			if (touchesCount > 0)
			{
				bool touchHandled = false;
				for (int i = 0; i < touchesCount && !touchHandled; i++)
				{
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began)
					{
						touchHandled = EventSystem.current.IsPointerOverGameObject() || CastTouchRay(touch.position, touch.radius); 
					}
				}
			}
		}

		private bool CastTouchRay(Vector2 i_TouchPosition, float i_TouchRadius)
		{
			bool handled = OnClickedOutsideUI(i_TouchPosition);

			if (Physics.SphereCast(playerCamera.ScreenPointToRay(i_TouchPosition), i_TouchRadius, out RaycastHit hit, 20))
			{
				ISelectable selectable = hit.transform.GetComponentInParent<ISelectable>();
				if (selectable != null)
				{
					selectable.Select();
					handled = true;
				}
			}

			return handled;
		}
	}
}