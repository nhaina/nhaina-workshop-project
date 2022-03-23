using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public class MovesFactory
	{
		public IBoardMove CreateTransformMove(Ship i_Target, ePipColor i_DestinationColor)
		{
			return new TransformMove() { DestinationColor = i_DestinationColor, TargetShip = i_Target };
		}

		public IBoardMove CreateFlyMove(Ship i_Target, IStar i_Destination)
		{
			return new FlyMove() { Destination = i_Destination, TargetShip = i_Target };
		}

		public IBoardMove CreateRaidMove(Ship i_Raider, Ship i_Targeted)
		{
			return new RaidMove() { RaiderShip = i_Raider, TargetedShip = i_Targeted };
		}

		public IBoardMove CreateBuildMove(Ship i_Target)
		{
			return new BuildMove() { TargetToClone = i_Target };
		}

		public IBoardMove CreateCatastropheMove(IStar i_Target, ePipColor i_CatastropheColor)
		{
			return new CatastropheMove() { TargetSystem = i_Target ,CatastropheColor = i_CatastropheColor };
		}

		public IBoardMove CreateSacrificeMove(Ship i_Target)
		{
			return new SacrificeMove() { TargetToSacrifice = i_Target };
		}
	}
}