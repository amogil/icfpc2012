using System;

namespace Logic
{
	public static class MapCellExtensions
	{
		public static bool IsTrampoline(this MapCell cell)
		{
			return cell >= MapCell.Trampoline1 && cell <= MapCell.Trampoline9;
		}

		public static bool IsTarget(this MapCell cell)
		{
			return cell >= MapCell.Target1 && cell <= MapCell.Target9;
		}
	}

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
			if (move == RobotMove.Right) return new Vector(1, 0);
			return new Vector(0, 0);
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