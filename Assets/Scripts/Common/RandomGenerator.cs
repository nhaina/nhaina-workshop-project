using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homeworlds.Common
{
	public static class RandomGenerator
	{
		private static readonly Random random = new Random();
		public static Random Random { get { return random; } }
	}
}
