namespace Homeworlds.Logic
{
	public class FlyMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }

		public Ship TargetShip { get; set; }
		public IStar Destination { get; set; }

		public void Execute()
		{
			BoardManager.MoveShip(TargetShip, Destination);
		}
	}
}