using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;

namespace Homeworlds.Logic
{
	public interface IBoardMoveVisitor
	{
		void Visit();
	}

	public interface IBuildMoveVisitor : IBoardMoveVisitor
	{
		void Visit(BuildMove move);
	}

	public interface IRaidMoveVisitor : IBoardMoveVisitor
	{
		void Visit(RaidMove move);
	}

	public interface ITransformMoveVisitor : IBoardMoveVisitor
	{
		void Visit(TransformMove move);
	}

	public interface IFlyMoveVisitor : IBoardMoveVisitor
	{
		void Visit(FlyMove move);
	}

	public interface ICatastropheMoveVisitor : IBoardMoveVisitor
	{
		void Visit(CatastropheMove move);
	}

	public interface ISacrificeMoveVisitor : IBoardMoveVisitor
	{
		void Visit(SacrificeMove move);
	}
}