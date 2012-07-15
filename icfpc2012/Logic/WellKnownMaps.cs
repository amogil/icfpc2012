using System;

namespace Logic
{
	public static class WellKnownMaps
	{
		public static Func<Map> Contest1 = () => LoadMap("contest1");
		public static Func<Map> TramTest = () => LoadMap("TramTest");
		public static Func<Map> Contest2 = () => LoadMap("contest2");
		public static Func<Map> Contest3 = () => LoadMap("contest3");
		public static Func<Map> Contest4 = () => LoadMap("contest4");
		// ...

		public static Map LoadMap(string wellKnownName)
		{
			return new Map(@"..\..\..\..\maps\" + wellKnownName + ".map.txt");
		}
	}
}