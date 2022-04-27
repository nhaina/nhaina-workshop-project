using Homeworlds.Logic;

namespace Homeworlds.AI
{
	public interface IAiStrategy
	{
		IBoardMove GetMove();
		BoardManager GameBoard { get; set; }
	}
}
