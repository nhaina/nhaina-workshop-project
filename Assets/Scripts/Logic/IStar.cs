using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public interface IStar : IEquatable<IStar>
	{
		int Identifier { get; }
		IEnumerable<Pip> Attributes { get; }
		void Accept(IAbstractStarVisitor visitor);
	}
}