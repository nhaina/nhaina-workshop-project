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

		public void Accept(IBoardMoveVisitor visitor)
		{
			if (visitor is IRaidMoveVisitor raidVisitor)
			{
				raidVisitor.Visit(this);
			}
			else
			{
				visitor.Visit();
			}
		}
	}
}