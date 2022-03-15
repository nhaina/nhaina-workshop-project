using Homeworlds.Common;
using System.Collections.Generic;

namespace Homeworlds.Logic
{
	public interface IStar
	{
		IEnumerable<ePipColor> Colors { get; }
		IEnumerable<ePipSize> Sizes { get; }
	}
}