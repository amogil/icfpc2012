using System;

namespace Logic
{
	public static class Extensions
	{
		public static MapCell[,] SkipBorder(this MapCell[,] map)
		{
			var res = new MapCell[map.GetLength(0) - 2, map.GetLength(1) - 2];
			for (int y = 1; y < map.GetLength(1) - 1; y++)
				for (int x = 1; x < map.GetLength(0) - 1; x++)
					res[x - 1, y - 1] = map[x, y];
			return res;
		}
		
		public static char ToChar(this RobotMove move)
		{
			if (move == RobotMove.Down) return 'D';
			if (move == RobotMove.Up) return 'U';
			if (move == RobotMove.Left) return 'L';
			if (move == RobotMove.Right) return 'R';
			if (move == RobotMove.Abort) return 'A';
			if (move == RobotMove.Wait) return 'W';
			throw new Exception(move.ToString());
		}
	}
}
