namespace Logic
{
	public static class MapExtensions
	{
		public static bool IsValidMoveWithoutMovingRocks(this IMap map, Vector from, Vector to)
		{
			var toCell = map[to];
			return toCell != MapCell.Rock && toCell != MapCell.Wall && toCell != MapCell.ClosedLift;
		}

		public static MapCell[,] SkipBorder(this MapCell[,] map)
		{
			var res = new MapCell[map.GetLength(0) - 2, map.GetLength(1) - 2];
			for (int y = 1; y < map.GetLength(1) - 1; y++)
				for (int x = 1; x < map.GetLength(0) - 1; x++)
					res[x - 1, y - 1] = map[x, y];
			return res;
		}

		public static long GetScore(this IMap map)
		{
			var c = 50;
			if (map.State == CheckResult.Win) c = 75;
			if (map.State == CheckResult.Fail) c = 25;
			return map.LambdasGathered * c - map.MovesCount;
		}
	}
}
