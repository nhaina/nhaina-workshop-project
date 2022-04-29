using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	[RequireComponent(typeof(Animator))]
	public class SceneStartTransitionListener : MonoBehaviour
	{
		[SerializeField]
		private SceneStartTransition transitionManager;
		[SerializeField]
		private string parameterName;
		[SerializeField]
		private bool isBlocking;
		[SerializeField]
		private int priority;
		private int parameterHash;
		private Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			parameterHash = Animator.StringToHash(parameterName);
			transitionManager.Started += TransitionManager_Started;
		}

		private void TransitionManager_Started(TransitionArgs obj)
		{
			if (obj.StartFlag)
			{
				StartAnimation();
			}
			else
			{
				obj.AnimationLength = animator.runtimeAnimatorController.animationClips[0].length;
				obj.IsBlocking = isBlocking;
				obj.Priority = priority;
			}
		}

		protected virtual void StartAnimation()
		{
			animator.SetTrigger(parameterHash);
		}
	}
}