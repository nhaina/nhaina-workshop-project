using Homeworlds.Common;
using System;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public interface IStar : IEquatable<IStar>
	{
		IEnumerable<ePipColor> Colors { get; }
		IEnumerable<ePipSize> Sizes { get; }
	}
}