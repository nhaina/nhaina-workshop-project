using System;
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
			/*manager = new Logic.BoardManager();
			manager.StartNewGame();
			Logic.Homeworld hw1 = new Logic.Homeworld(Common.ePipColor.Red, Common.ePipSize.Large, Common.ePipColor.Blue, Common.ePipSize.Medium, Common.ePlayer.Player1, false);
			Logic.Homeworld hw2 = new Logic.Homeworld(Common.ePipColor.Yellow, Common.ePipSize.Small, Common.ePipColor.Blue, Common.ePipSize.Medium, Common.ePlayer.Player2, false);
			manager.SetupRound(hw1, new Logic.Ship(new Common.Pip(Common.ePipColor.Green, Common.ePipSize.Large), Common.ePlayer.Player1, hw1),
							hw2, new Logic.Ship(new Common.Pip(Common.ePipColor.Green, Common.ePipSize.Large), Common.ePlayer.Player2, hw2));*/
		}

		/*private void OnEnable()
		{
			viewBoard = FindObjectOfType<ViewBoard>();
			viewBoard.SelectStarCallback = onSelectedStar;
			viewBoard.SelectShipCallback = onSelectedShip;
			viewBoard.UpdateField(manager.CurrentState);
		}*/

		private void Update()
		{
			if (Input.touchSupported)
			{
				getTouchInput();
			}
			else
			{
				getMouseInput();
			}
			if (Input.GetButton("Cancel"))
			{
				Application.Quit();
			}
		}

		private void getMouseInput()
		{
			if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
			{
				CastTouchRay(Input.mousePosition, 0.25f);
			}
		}

		protected bool OnClickedOutsideUI(Vector2 i_ScreenPositionClicked)
		{
			CancellationRequest request = new CancellationRequest();
			ClickedOutsideUI?.Invoke(i_ScreenPositionClicked, request);
			return request.RequestCancel;
		}

		private void getTouchInput()
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