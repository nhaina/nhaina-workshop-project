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
		private MainController controller;

		private void Awake()
		{
			controller = new MainController();
			controller.BoardManager = new Logic.BoardManager();
		}

		private void OnEnable()
		{
			controller.View = viewBoard;
			controller.GUI = guiManager;
		}

		private void Start()
		{
			controller.StartGame();
		}
	}
}