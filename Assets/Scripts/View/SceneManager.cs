using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Homeworlds.View
{
	public class SceneManager : MonoBehaviour
	{
		public bool CanLoadMenuScene { get; set; }
		private void Start()
		{
			CanLoadMenuScene = false;
			StartCoroutine(loadMenuScene());
		}

		private IEnumerator loadMenuScene()
		{
			yield return new WaitUntil(() => CanLoadMenuScene);
			yield return USceneManager.LoadSceneAsync(1);
		}
	}
}