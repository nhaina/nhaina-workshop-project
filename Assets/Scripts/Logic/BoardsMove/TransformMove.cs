using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public class TransformMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }
		public Ship TargetShip { get; set; }
		public ePipColor DestinationColor { get; set; }

		public TransformMove()
		{ }

		public void Execute()
		{
			BoardManager.TransformShip(TargetShip, DestinationColor);
		}
	}
}