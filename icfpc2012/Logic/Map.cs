using System;
using System.Linq;

namespace Logic
{
	public enum RobotMove
	{
		Left,
		Right,
		Up,
		Down,
		Wait,
		Abort
	}

	public enum MapCell
	{
		Empty,
		Earth,
		Rock,
		Lambda,
		Wall,
		Robot,
		ClosedLift,
		OpenedLift
	}

	public enum CheckResult
	{
		Nothing,
		Win,
		Fail
	}

	public class Map
	{
		public readonly int Height;
		public readonly int Width;
		public CheckResult State = CheckResult.Nothing;
		private int lambdaCounter;
		private MapCell[,] map;
		private MapCell[,] swapMap;

		private int totalLambdaCount;

		public Map(string[] lines)
		{
			lambdaCounter = 0;

			Height = lines.Length;
			Width = lines.Max(a => a.Length);

			map = new MapCell[Width + 2,Height + 2];
			swapMap = new MapCell[Width + 2,Height + 2];

			for (int row = 0; row < Height + 2; row++)
			{
				for (int col = 0; col < Width + 2; col++)
				{
					map[col, row] = MapCell.Wall;
					swapMap[col, row] = MapCell.Wall;
				}
			}

			for (int row = 1; row < Height + 1; row++)
			{
				for (int col = 1; col < Width + 1; col++)
				{
					map[col, row] = MapCell.Empty;
					swapMap[col, row] = MapCell.Empty;
				}
			}

			for (int row = 0; row < Height; row++)
			{
				for (int col = 0; col < Width; col++)
				{
					int newY = Height - row - 1;

					string line = lines[row].PadRight(Width, ' ');
					map[col + 1, newY + 1] = Parse(line[col]);
					if (map[col + 1, newY + 1] == MapCell.Robot)
					{
						RobotX = col + 1;
						RobotY = newY + 1;
					}
					if (map[col + 1, newY + 1] == MapCell.Lambda)
						lambdaCounter++;
				}
			}

			Height += 2;
			Width += 2;

			totalLambdaCount = lambdaCounter;
		}

		public int RobotX { get; private set; }
		public int RobotY { get; private set; }

		public MapCell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public override string ToString()
		{
			return new MapSerializer().Serialize(map.SkipBorder());
		}

		private static MapCell Parse(char c)
		{
			switch (c)
			{
				case '#':
					return MapCell.Wall;
				case '*':
					return MapCell.Rock;
				case '\\':
					return MapCell.Lambda;
				case '.':
					return MapCell.Earth;
				case ' ':
					return MapCell.Empty;
				case 'L':
					return MapCell.ClosedLift;
				case 'O':
					return MapCell.OpenedLift;
				case 'R':
					return MapCell.Robot;
			}

			throw new Exception("InvalidMap " + c);
		}

		public Map Move(RobotMove move)
		{
			if (move == RobotMove.Abort)
			{
				State = CheckResult.Win;
				return this;
			}

			if (State != CheckResult.Nothing)
				throw new GameFinishedException();

			if (move != RobotMove.Wait)
			{
				int newRobotX = RobotX;
				int newRobotY = RobotY;

				if (move == RobotMove.Up) newRobotY++;
				if (move == RobotMove.Down) newRobotY--;
				if (move == RobotMove.Left) newRobotX--;
				if (move == RobotMove.Right) newRobotX++;

				if (!CheckValid(newRobotX, newRobotY))
					throw new NoMoveException();

				DoMove(newRobotX, newRobotY);
			}
			Update();

			return this;
		}

		private bool CheckValid(int newRobotX, int newRobotY)
		{
			if (map[newRobotX, newRobotY] == MapCell.Wall || map[newRobotX, newRobotY] == MapCell.ClosedLift)
				return false;

			if (map[newRobotX, newRobotY] != MapCell.Rock)
				return true;

			if (newRobotX - RobotX == 0)
				return false;

			int checkX = newRobotX*2 - RobotX;

			if (map[checkX, RobotY] == MapCell.Empty)
				return true;

			return false;
		}

		private void DoMove(int newRobotX, int newRobotY)
		{
			if (map[newRobotX, newRobotY] == MapCell.Lambda)
				lambdaCounter--;
			else if (map[newRobotX, newRobotY] == MapCell.Earth)
			{
			}
			else if (map[newRobotX, newRobotY] == MapCell.OpenedLift)
			{
				State = CheckResult.Win;
			}
			else if (map[newRobotX, newRobotY] == MapCell.Rock)
			{
				int rockX = newRobotX*2 - RobotX;
				map[rockX, newRobotY] = MapCell.Rock;
			}
			map[RobotX, RobotY] = MapCell.Empty;
			map[newRobotX, newRobotY] = MapCell.Robot;

			RobotX = newRobotX;
			RobotY = newRobotY;
		}

		private void Update()
		{
			for (int y = 1; y < Height - 1; y++)
			{
				for (int x = 1; x < Width - 1; x++)
				{
					swapMap[x, y] = map[x, y];

					bool rockFall = false;
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x, y - 1] = MapCell.Rock;
						CheckRobotDanger(x, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
					    && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x + 1, y] = MapCell.Empty;
						swapMap[x + 1, y - 1] = MapCell.Rock;
						CheckRobotDanger(x + 1, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
					    && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
					    && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x - 1, y] = MapCell.Empty;
						swapMap[x - 1, y - 1] = MapCell.Rock;
						CheckRobotDanger(x - 1, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
					    && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x + 1, y] = MapCell.Empty;
						swapMap[x + 1, y - 1] = MapCell.Rock;
						CheckRobotDanger(x + 1, y - 1);
					}
					if (map[x, y] == MapCell.ClosedLift && lambdaCounter == 0)
					{
						swapMap[x, y] = MapCell.OpenedLift;
					}
				}
			}

			MapCell[,] swap = swapMap;
			swapMap = map;
			map = swap;
		}

		private void CheckRobotDanger(int x, int y)
		{
			if (map[x, y - 1] == MapCell.Robot)
			{
				State = CheckResult.Fail;
				throw new KilledByRockException();
			}
		}
	}

	public class NoMoveException : Exception
	{
	}

	public class GameFinishedException : Exception
	{
	}

	public class KilledByRockException : GameFinishedException
	{
	}
}