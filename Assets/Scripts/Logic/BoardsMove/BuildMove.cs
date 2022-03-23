using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public class BuildMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }

		public Ship TargetToClone { get; set; }

		public void Execute()
		{
			BoardManager.BuildShip(TargetToClone);
		}
	}
}