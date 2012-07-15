using System;
using System.Linq;

namespace Logic
{
	public static class MapExtensions
	{
		public static bool IsRock(this MapCell cell)
		{
			return cell == MapCell.Rock || cell == MapCell.LambdaRock;
		}

		public static bool IsMovable(this MapCell cell)
		{
			return cell == MapCell.Empty || cell == MapCell.Earth || cell == MapCell.Robot;
		}

		public static bool IsRockMovable(this MapCell cell)
		{
			return cell == MapCell.Empty || cell == MapCell.Robot;
		}

		public static bool IsValidMoveWithoutMovingRocks(this Map map, Vector from, Vector to)
		{
			var toCell = map.GetCell(to);
			return toCell == MapCell.Beard || toCell.IsTrampoline() || toCell == MapCell.OpenedLift || toCell == MapCell.Lambda || toCell == MapCell.Empty || toCell == MapCell.Earth || toCell == MapCell.Robot || toCell == MapCell.Razor;
		}

		public static MapCell[,] SkipBorder(this Map map)
		{
			var field = new MapCell[map.Width - 2, map.Height - 2];
			for (int y = 1; y < map.Height - 1; y++)
				for (int x = 1; x < map.Width - 1; x++)
					field[x - 1, y - 1] = map.GetCell(x, y);
			return field;
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


		private static bool IsEmptyOrRobot(MapCell cell)
		{
			return cell == MapCell.Empty || cell == MapCell.Robot;
		}

		public static Vector TryToMoveRock(this Map map, Vector p, params Vector[] emptyCells)
		{
			Func<int, int, bool> isEmpty =
				(xx, yy) => emptyCells.Contains(new Vector(xx, yy)) || IsEmptyOrRobot(map.GetCell(xx, yy));
			var x = p.X;
			var y = p.Y;
			if (map.GetCell(x, y).IsRock())
			{

				if (isEmpty(x, y - 1))
				{
					return new Vector(x, y - 1);
				}
				MapCell upCell = map.GetCell(x, y - 1);
				if (upCell.IsRock()
				    && isEmpty(x + 1, y) && isEmpty(x + 1, y - 1))
				{
					return new Vector(x + 1, y - 1);
				}
				if (upCell.IsRock()
				    && (!isEmpty(x + 1, y) || !isEmpty(x + 1, y - 1))
				    && isEmpty(x - 1, y) && isEmpty(x - 1, y - 1))
				{
					return new Vector(x - 1, y - 1);
				}
				if (upCell == MapCell.Lambda
				    && isEmpty(x + 1, y) && isEmpty(x + 1, y - 1))
				{
					return new Vector(x + 1, y - 1);
				}
			}

			return p;
		}

		public static bool IsSafeMove(this Map map, Vector from, Vector to, int movesDone, int waterproofLeft)
		{
			if (waterproofLeft <= 0 && map.WaterLevelAfterUpdate(map.MovesCount + movesDone - 1) >= to.Y)
				return false;

			var emptyCells = new[] {from, to};

			bool isSafe = true;

			if (to.Y == from.Y - 1)
			{
				for (int x = to.X - 1; x <= to.X + 1; x++)
				{
					var newPosition = map.TryToMoveRock(new Vector(x, to.Y + 2), emptyCells);
					if (newPosition.X == to.X && newPosition.Y == to.Y + 1)
						isSafe = false;
				}
			}

			if (to.Y + movesDone + 1 < map.Height)//камни сверху
			{
				int y = to.Y + movesDone + 1;
				for (int x = to.X - 1; x <= to.X + 1; x++)
				{
					var newPosition = map.TryToMoveRock(new Vector(x, y), emptyCells);

					if (newPosition.X == to.X && newPosition.Y == y - 1 && map.IsColumnEmpty(to.X, to.Y + 1, y - 2))
						isSafe = false;
				}
			}

			return isSafe;
		}

		public static bool IsColumnEmpty(this Map map, int x, int bottomY, int topY)
		{
			for (int y = bottomY; y <= topY; y++)
			{
				if (map.GetCell(x, y) != MapCell.Empty)
					return false;
			}
			return true;
		}

		public static Vector GetTrampolineTarget(this Map map, Vector trampolineOrJustCell)
		{
			if (!map.GetCell(trampolineOrJustCell).IsTrampoline()) return trampolineOrJustCell;
			return map.Targets[map.TrampToTarget[map.GetCell(trampolineOrJustCell)]];
		}
	}
}
