using System.Collections.Generic;

namespace Logic
{
	public static class MapExtensions
	{
		public static bool IsMovable(this MapCell cell)
		{
			return cell == MapCell.Empty || cell == MapCell.Earth || cell == MapCell.Robot;
		}

		public static bool IsValidMoveWithoutMovingRocks(this Map map, Vector from, Vector to)
		{
			var toCell = map[to];
			return toCell.IsTrampoline() || toCell == MapCell.OpenedLift || toCell == MapCell.Lambda || toCell == MapCell.Empty || toCell == MapCell.Earth || toCell == MapCell.Robot;
		}

		public static MapCell[,] SkipBorder(this MapCell[,] map)
		{
			var res = new MapCell[map.GetLength(0) - 2, map.GetLength(1) - 2];
			for (int y = 1; y < map.GetLength(1) - 1; y++)
				for (int x = 1; x < map.GetLength(0) - 1; x++)
					res[x - 1, y - 1] = map[x, y];
			return res;
		}

		public static long GetScore(this Map map)
		{
			var c = 50;
			if (map.State == CheckResult.Win) c = 75;
			if (map.State == CheckResult.Fail) c = 25;
			return map.LambdasGathered * c - map.MovesCount;
		}

		public static bool RocksFallAfterMoveTo(this Map map, Vector to)
		{
			for (int rockX = to.X - 1; rockX <= to.X + 1; rockX++)
			{
				for (int rockY = to.Y; rockY <= to.Y + 1; rockY++)
				{
					var coords = new Vector(rockX, rockY);
					if (!coords.Equals(map.TryToMoveRock(coords)))
						return true;
				}
			}
			return false;
		}

		
		public static Vector TryToMoveRock(this Map map, Vector coords)
		{
			return TryToMoveRock(map, coords.X, coords.Y);
		}

		private static bool IsEmptyOrRobot(MapCell cell)
		{
			return cell == MapCell.Empty || cell == MapCell.Robot;
		}

		public static Vector TryToMoveRock(this Map map, int x, int y)
		{
			if (map[x, y] == MapCell.Rock && IsEmptyOrRobot(map[x, y - 1]))
			{
				return new Vector(x, y - 1);
			}
			if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
				&& IsEmptyOrRobot(map[x + 1, y]) && IsEmptyOrRobot(map[x + 1, y - 1]))
			{
				return new Vector(x + 1, y - 1);
			}
			if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
				&& (!IsEmptyOrRobot(map[x + 1, y]) || !IsEmptyOrRobot(map[x + 1, y - 1]))
				&& IsEmptyOrRobot(map[x - 1, y]) && IsEmptyOrRobot(map[x - 1, y - 1]))
			{
				return new Vector(x - 1, y - 1);
			}
			if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
				&& IsEmptyOrRobot(map[x + 1, y]) && IsEmptyOrRobot(map[x + 1, y - 1]))
			{
				return new Vector(x + 1, y - 1);
			}

			return new Vector(x, y);
		}

	}
}
