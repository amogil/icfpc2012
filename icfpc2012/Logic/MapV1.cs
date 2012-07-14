using System;
using System.Linq;

namespace Logic
{
	public class MapV1 : IMap
	{
		public int MovesCount { get; private set; }
		public int LambdasGathered { get; private set; }
		public CheckResult State { get; private set; }

		public int Height { get; private set; }
		public int Width { get; private set; }
		private MapCell[,] map;
		private MapCell[,] swapMap;

		public int TotalLambdaCount { get; private set; }
		public int Water { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int StepsToIncreaseWater { get; private set; }
		public int WaterproofLeft { get; private set; }

		public MapV1(string[] lines)
		{
			State = CheckResult.Nothing;

			int firstBlankLineIndex = Array.IndexOf(lines, "");
			Height = firstBlankLineIndex == -1 ? lines.Length : firstBlankLineIndex;
			Width = lines.Max(a => a.Length);

			map = new MapCell[Width + 2, Height + 2];
			swapMap = new MapCell[Width + 2, Height + 2];

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
					{
						TotalLambdaCount++;
					}
				}
			}
			InitializeFlooding(lines.Skip(Height + 1).ToArray());

			Height += 2;
			Width += 2;
		}

		private void InitializeFlooding(string[] floodingSpecs)
		{
			Water = 0;
			Flooding = 0;
			Waterproof = 10;
			foreach (var floodingSpec in floodingSpecs)
			{
				string[] parts = floodingSpec.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts[0] == "Water") Water = int.Parse(parts[1]);
				if (parts[0] == "Flooding") Flooding = int.Parse(parts[1]);
				if (parts[0] == "Waterproof") Waterproof = int.Parse(parts[1]);
			}
			StepsToIncreaseWater = Flooding;
			WaterproofLeft = Waterproof;
		}

		public Vector Robot { get { return new Vector(RobotX, RobotY); } }
		public int RobotX { get; private set; }
		public int RobotY { get; private set; }

		public MapCell this[Vector pos]
		{
			get { return map[pos.X, pos.Y]; }
		}

		public MapCell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public override string ToString()
		{
			return new MapSerializer().Serialize(map.SkipBorder(), Water, Flooding, Waterproof);
		}

		public string GetMapStateAsAscii()
		{
			return new MapSerializer().SerializeMapOnly(map.SkipBorder()).ToString();
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

		public IMap Move(RobotMove move)
		{
			if (move == RobotMove.Abort)
			{
				State = CheckResult.Abort;
				return this;
			}

			if (State != CheckResult.Nothing)
				throw new GameFinishedException();

			MovesCount++;
			if (move != RobotMove.Wait)
			{
				int newRobotX = RobotX;
				int newRobotY = RobotY;

				if (move == RobotMove.Up) newRobotY++;
				if (move == RobotMove.Down) newRobotY--;
				if (move == RobotMove.Left) newRobotX--;
				if (move == RobotMove.Right) newRobotX++;

				if (!CheckValid(newRobotX, newRobotY))
				{
					Update();
					throw new NoMoveException();
				}

				DoMove(newRobotX, newRobotY);
			}

			if (State != CheckResult.Win)
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

			int checkX = newRobotX * 2 - RobotX;

			if (map[checkX, RobotY] == MapCell.Empty)
				return true;

			return false;
		}

		private void DoMove(int newRobotX, int newRobotY)
		{
			if (map[newRobotX, newRobotY] == MapCell.Lambda)
			{
				LambdasGathered++;
			}
			else if (map[newRobotX, newRobotY] == MapCell.Earth)
			{
			}
			else if (map[newRobotX, newRobotY] == MapCell.OpenedLift)
			{
				State = CheckResult.Win;
			}
			else if (map[newRobotX, newRobotY] == MapCell.Rock)
			{
				int rockX = newRobotX * 2 - RobotX;
				map[rockX, newRobotY] = MapCell.Rock;
			}

			map[RobotX, RobotY] = MapCell.Empty;
			if (State != CheckResult.Win)
			{
				map[newRobotX, newRobotY] = MapCell.Robot;

				RobotX = newRobotX;
				RobotY = newRobotY;
			}
		}

		private void Update()
		{
			var robotFailed = false;
			for (int y = 1; y < Height - 1; y++)
			{
				for (int x = 1; x < Width - 1; x++)
				{
					swapMap[x, y] = map[x, y];

					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x, y - 1] = MapCell.Rock;
						robotFailed |= IsRobotKilledByRock(x, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
						&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x + 1, y] = MapCell.Empty;
						swapMap[x + 1, y - 1] = MapCell.Rock;
						robotFailed |= IsRobotKilledByRock(x + 1, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
						&& (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
						&& map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x - 1, y] = MapCell.Empty;
						swapMap[x - 1, y - 1] = MapCell.Rock;
						robotFailed |= IsRobotKilledByRock(x - 1, y - 1);
					}
					if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
						&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						swapMap[x, y] = MapCell.Empty;
						swapMap[x + 1, y] = MapCell.Empty;
						swapMap[x + 1, y - 1] = MapCell.Rock;
						robotFailed |= IsRobotKilledByRock(x + 1, y - 1);
					}
					if (map[x, y] == MapCell.ClosedLift && LambdasGathered == TotalLambdaCount)
					{
						swapMap[x, y] = MapCell.OpenedLift;
					}
				}
			}

			var swap = swapMap;
			swapMap = map;
			map = swap;
			robotFailed |= IsRobotKilledByFlood();

			if (robotFailed)
			{
				State = CheckResult.Fail;
				throw new GameFinishedException();
			}
		}

		private bool IsRobotKilledByFlood()
		{
			if (Water >= RobotY) WaterproofLeft--;
			else WaterproofLeft = Waterproof;
			if (WaterproofLeft <= 0) return true;
			if (Flooding > 0)
			{
				StepsToIncreaseWater--;
				if (StepsToIncreaseWater == 0)
				{
					Water++;
					StepsToIncreaseWater = Flooding;
				}
			}
			return false;
		}

		private bool IsRobotKilledByRock(int x, int y)
		{
			return map[x, y - 1] == MapCell.Robot;
		}

		public bool LoadPreviousState()
		{
			return false;
		}
	}
}