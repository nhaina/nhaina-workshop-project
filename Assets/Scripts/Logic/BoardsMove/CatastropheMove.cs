using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public class CatastropheMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }

		public IStar TargetSystem { get; set; }
		public ePipColor CatastropheColor { get; set; }

		public void Execute()
		{
			BoardManager.DeclareCatastrophe(TargetSystem, CatastropheColor);
		}

		public void Accept(IBoardMoveVisitor visitor)
		{
			if (visitor is ICatastropheMoveVisitor catVisitor)
			{
				catVisitor.Visit(this);
			}
			else
			{
				visitor.Visit();
			}
		}

	}
}