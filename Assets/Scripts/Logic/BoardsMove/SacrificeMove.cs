using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public class SacrificeMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }

		public Ship TargetToSacrifice { get; set; }

		public void Execute()
		{
			BoardManager.SacrificeShip(TargetToSacrifice);
		}

		public void Accept(IBoardMoveVisitor visitor)
		{
			if (visitor is ISacrificeMoveVisitor sacrificeVisitor)
			{
				sacrificeVisitor.Visit(this);
			}
			else
			{
				visitor.Visit();
			}
		}
	}
}