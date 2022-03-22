using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class CancellationRequest
	{
		private bool m_CancelRequested;

		public CancellationRequest()
			:this(false)
		{ }

		public CancellationRequest(bool i_RequestCancel)
		{
			m_CancelRequested = i_RequestCancel;
		}

		public bool RequestCancel
		{
			get { return m_CancelRequested; }
			set { m_CancelRequested |= value; }
		}

	}
}
