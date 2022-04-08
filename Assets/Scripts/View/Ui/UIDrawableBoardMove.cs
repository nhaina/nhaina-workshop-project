using Homeworlds.Logic;

namespace Homeworlds.View
{
	internal class UIDrawableBoardMove : IUIDrawable
	{
		private class DrawableMoveVisitor : IBoardMoveVisitor, IRaidMoveVisitor, IBuildMoveVisitor,
											ITransformMoveVisitor, IFlyMoveVisitor, ISacrificeMoveVisitor, ICatastropheMoveVisitor
		{
			public string Content { get; private set; }
			public ViewBoard GameViewBoard { get; set; }

			public void Visit()
			{
				Content = "Unknown Move";
			}

			public void Visit(RaidMove move)
			{
				Content = $"Raid {move.TargetedShip.Attributes}";
			}

			public void Visit(BuildMove move)
			{
				Content = $"Clone";
			}

			public void Visit(TransformMove move)
			{
				Content = $"Transform\nto {move.DestinationColor}";
			}

			public void Visit(FlyMove move)
			{
				string destDescription, identifier;
				if (move.Destination is Homeworld)
				{
					destDescription = $"{string.Join("-", move.Destination.Attributes)} Homeworld";
					identifier = string.Empty;
				}
				else
				{
					destDescription = string.Join(" ", move.Destination.Attributes);
					identifier = move.BoardManager.IsKnownStarOrHomeworld(move.Destination) ? GameViewBoard.GetStarName(move.Destination.Identifier) : "New";
				}
				Content = $"Move to\n {identifier} {destDescription}";
			}

			public void Visit(SacrificeMove move)
			{
				int actionsWorth = 1 + (int)move.TargetToSacrifice.Size;
				Content = $"Sacrifice ({actionsWorth})";
			}

			public void Visit(CatastropheMove move)
			{
				Content = $"Declare\n{move.CatastropheColor}\nCatastrophe";
			}
		}

		public UIDrawableBoardMove()
		{ }

		public string Content
		{
			get
			{
				DrawableMoveVisitor visitor = new DrawableMoveVisitor() { GameViewBoard = GameViewBoard };
				BoardMove.Accept(visitor);
				return visitor.Content;
			}
		}

		public ViewBoard GameViewBoard { get; set; }
		public IBoardMove BoardMove { get; set; }
	}
}