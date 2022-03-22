using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private Camera playerCamera;
		public Camera PlayerCamera { get { return playerCamera; } set { playerCamera = value; } }

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
			if (Input.GetMouseButtonDown(0))
			{
				CastTouchRay(Input.mousePosition, 0.5f);
			}
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
						touchHandled = CastTouchRay(touch.position, touch.radius);
					}
				}
			}
		}

		private bool CastTouchRay(Vector2 i_TouchPosition, float i_TouchRadius)
		{
			bool handled = false;
			if (Physics.SphereCast(playerCamera.ScreenPointToRay(i_TouchPosition), i_TouchRadius, out RaycastHit hit, 20	))
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