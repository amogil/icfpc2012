using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
	static class FloodChecker
	{
		static int GetLevel(int water, int flooding, int time)
		{
			return water + time/flooding;
		}
	}
}
