namespace Homeworlds.Logic
{
	public class RaidMove : IBoardMove
	{
		public BoardManager BoardManager { get; set; }
		public Ship RaiderShip { get; set; }
		public Ship TargetedShip { get; set; }

		public RaidMove()
		{ }

		public void Execute()
		{
			BoardManager.RaidShip(RaiderShip, TargetedShip);
		}
	}
}