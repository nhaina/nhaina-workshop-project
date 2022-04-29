using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Homeworlds.View
{
	public class TransitionArgs
	{
		public int Priority { get; set; }
		public bool IsBlocking { get; set; }
		public float AnimationLength { get; set; }
		public bool StartFlag { get; set; }

		public class PriorityComprarer : IComparer<TransitionArgs>
		{
			public int Compare(TransitionArgs x, TransitionArgs y)
			{
				return x.Priority.CompareTo(y.Priority);
			}
		}
	}

	[RequireComponent(typeof(Canvas))]
	public class SceneStartTransition : MonoBehaviour
	{
		public event Action<TransitionArgs> Started;

		private void Awake()
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}

		private IEnumerator Start()
		{
			Delegate[] invocations = Started.GetInvocationList();
			TransitionArgs[] args = warmUpQuery(invocations);
			int idx = -1;
			float maxLength = 0;
			foreach (Action<TransitionArgs> invocation in invocations)
			{
				args[++idx].StartFlag = true;
				invocation.Invoke(args[idx]);
				if (args[idx].IsBlocking)
				{
					maxLength = 0;
					yield return new WaitForSeconds(args[idx].AnimationLength);
				}
				else
				{
					maxLength = Mathf.Max(args[idx].AnimationLength, maxLength);
				}
			}
			yield return new WaitForSeconds(maxLength);
		}

		private TransitionArgs[] warmUpQuery(Delegate[] invocations)
		{
			TransitionArgs[] args = new TransitionArgs[invocations.Length];
			int idx = -1;
			foreach (Action<TransitionArgs> invocation in invocations)
			{
				args[++idx] = new TransitionArgs() { StartFlag = false };
				invocation.Invoke(args[idx]);
			}
			Array.Sort(args, invocations, 0, args.Length, new TransitionArgs.PriorityComprarer());
			return args;
		}
	}
}