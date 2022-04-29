using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class CancellationRequest
	{
		private bool cancelRequested;

		public CancellationRequest()
			:this(false)
		{ }

		public CancellationRequest(bool i_RequestCancel)
		{
			cancelRequested = i_RequestCancel;
		}

		public bool RequestCancel
		{
			get { return cancelRequested; }
			set { cancelRequested |= value; }
		}

	}
}
