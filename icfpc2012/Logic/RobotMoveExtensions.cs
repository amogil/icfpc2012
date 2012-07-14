using System;

namespace Logic
{
	public static class RobotMoveExtensions
	{
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

		public static Vector ToVector(this RobotMove move)
		{
			if (move == RobotMove.Down) return new Vector(0, -1);
			if (move == RobotMove.Up) return new Vector(0, 1);
			if (move == RobotMove.Left) return new Vector(-1, 0);
			if (move == RobotMove.Right) return new Vector(0, 1);
			return new Vector(0, 0);
		}
	}
}