using Homeworlds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	internal class UIDrawablePip : IUIDrawable
	{
		public UIDrawablePip()
		{ }
		public string Content { get { return Pip.ToString().Replace(" ", Environment.NewLine); } }
		public Pip Pip { get; set; }
	}
}