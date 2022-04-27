using Homeworlds.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.Controller
{
	public class GameController : MonoBehaviour
	{
		[SerializeField]
		private ViewBoard viewBoard;
		[SerializeField]
		private GUIManager guiManager;
		[SerializeField]
		private PlayerController playerController;
		private MainController controller;
		private AI.AiPlayer player;

		private void Awake()
		{
			controller = new MainController();
			controller.BoardManager = new Logic.BoardManager();
			/*player = new AI.AiPlayer()
			{
				Strategy = new AI.AlphaBetaCutoffMinimax(),
				Player = Common.ePlayer.Player2
			};*/
		}

		private void OnEnable()
		{
			controller.View = viewBoard;
			controller.GUI = guiManager;
			controller.BoardUpdated += controller_InitialBoardUpdated;
			controller.BoardManager.TurnEnded += BoardManager_TurnEnded;
		}

		private void BoardManager_TurnEnded()
		{
			Debug.Log($"Turn Ended! ActivePlayer: {controller.BoardManager.ActivePlayer}");
		}

		private void controller_InitialBoardUpdated(Logic.BoardState obj)
		{
			playerController.ClickedOutsideUI += playerController_ClickedOutsideUI;
			controller.BoardUpdated -= controller_InitialBoardUpdated;
		}

		private void playerController_ClickedOutsideUI(Vector2 arg1, CancellationRequest arg2)
		{
			if (guiManager.IsUIOpen)
			{
				arg2.RequestCancel = true;
				guiManager.CloseUI();
			}
		}

		private void Start()
		{
			controller.StartGame();
			//player.GameBoard = controller.BoardManager;
		}
	}
}