﻿namespace Logic
{
	public static class Extensions
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
	}
}
