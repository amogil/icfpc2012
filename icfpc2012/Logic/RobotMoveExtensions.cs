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
			if (move == RobotMove.CutBeard) return 'S';
			throw new Exception(move.ToString());
		}

		private static readonly Vector up = new Vector(0, 1);
		private static readonly Vector down = new Vector(0, -1);
		private static readonly Vector left = new Vector(-1, 0);
		private static readonly Vector right = new Vector(1, 0);
		private static readonly Vector zero = new Vector(0, 0);

		public static Vector ToVector(this RobotMove move)
		{
			if (move == RobotMove.Down) return down;
			if (move == RobotMove.Up) return up;
			if (move == RobotMove.Left) return left;
			if (move == RobotMove.Right) return right;
			return zero;
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
				case 'S':
					return RobotMove.CutBeard;
			}
			throw new Exception(move.ToString());
		}
	}
}