using System;

namespace Logic
{
	public static class Extensions
	{
		public static bool IsValidMoveWithoutMovingRocks(this Map map, Vector from, Vector to)
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

		public static RobotMove ToRobotMove(this char move)
		{
			switch (move)
			{
				case 'D':
					return RobotMove.Down;
				case 'U':
					return RobotMove.Up;
				case 'L':
					return RobotMove.Left;
				case 'R':
					return RobotMove.Right;
				case 'W':
					return RobotMove.Wait;
				case 'A':
					return RobotMove.Abort;
			}
			throw new Exception(move.ToString());
		}
	}
}
