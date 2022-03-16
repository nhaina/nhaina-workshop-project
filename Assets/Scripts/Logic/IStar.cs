using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public interface IStar : IEquatable<IStar>
	{
		IEnumerable<Pip> Attributes { get; }
	}
}