using System;
using System.Collections.Generic;
using System.Linq;

namespace Homeworlds.Logical
{
	public class LogicManager
	{
		public IResourcesManager ResourcesManager { get; set; }
		public IBoardManager BoardManager { get; set; }

		public LogicManager()
		{ }
	}
}