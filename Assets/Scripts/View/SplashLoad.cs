using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Homeworlds.View
{
	public class SplashLoad : MonoBehaviour
	{
		[SerializeField]
		private Animator splashAnimator;
		[SerializeField]
		private float waitBeforePlaying = 2;
		[SerializeField]
		private string splashAnimationKey = "TriggerSplash";
		[SerializeField]
		private SceneManager conductor;
		private float splashAnimationLength;
		private int splashAnimationKeyHash;

		private void Awake()
		{
			splashAnimationKeyHash = Animator.StringToHash(splashAnimationKey);
		}

		private IEnumerator Start()
		{
			initializeAnimationParameters();
			yield return new WaitForSeconds(waitBeforePlaying);
			splashAnimator.SetTrigger(splashAnimationKeyHash);
			yield return new WaitForSeconds(splashAnimationLength);
			conductor.CanLoadMenuScene = true;
		}

		private void initializeAnimationParameters()
		{
			if (splashAnimator == null || conductor == null)
			{
				throw new MissingComponentException();
			}
			conductor.CanLoadMenuScene = false;
			splashAnimationLength = splashAnimator.runtimeAnimatorController.animationClips
				.Max(clip => clip.length);
		}
	}
}