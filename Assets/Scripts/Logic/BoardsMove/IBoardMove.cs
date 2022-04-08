using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public interface IBoardMove
	{
		BoardManager BoardManager { get; set; }

		void Execute();
		void Accept(IBoardMoveVisitor visitor);
	}
}