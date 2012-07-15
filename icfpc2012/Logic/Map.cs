using System;
using System.Collections.Generic;
using System.IO;
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
		Abort,
		CutBeard
	}

	public enum MapCell
	{
		Empty = ' ',
		Earth = '.',
		Rock = '*',
		Lambda = '\\',
		Wall = '#',
		Robot = 'R',
		Beard = 'W',
		Razor = '!',
		LambdaRock = '@',
		ClosedLift = 'L',
		OpenedLift = 'O',
		Trampoline1 = 'A',
		Trampoline2 = 'B',
		Trampoline3 = 'C',
		Trampoline4 = 'D',
		Trampoline5 = 'E',
		Trampoline6 = 'F',
		Trampoline7 = 'G',
		Trampoline8 = 'H',
		Trampoline9 = 'I',
		Target1 = '1',
		Target2 = '2',
		Target3 = '3',
		Target4 = '4',
		Target5 = '5',
		Target6 = '6',
		Target7 = '7',
		Target8 = '8',
		Target9 = '9'
	}
	
	public enum CheckResult
	{
		Nothing,
		Win,
		Fail,
		Abort,
	}

	public class Map
	{
		public string[] Lines { get; private set; }

		public Vector GetTrampolineTarget(Vector trampolineOrJustCell)
		{
			if (!this[trampolineOrJustCell].IsTrampoline()) return trampolineOrJustCell;
			else return Targets[TrampToTarget[this[trampolineOrJustCell]]];
		}

		public int Razors { get; private set; }
		public int Growth { get; private set; }
		public int GrowthLeft { get; private set; }

		private Dictionary<MapCell, Vector> Targets = new Dictionary<MapCell, Vector>();
		private Dictionary<MapCell, Vector> Trampolines = new Dictionary<MapCell, Vector>();
		private Dictionary<MapCell, MapCell> TrampToTarget = new Dictionary<MapCell, MapCell>();

		private static HashSet<char> TrampolinesChars = new HashSet<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
		private static HashSet<char> TargetsChars = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		public int MovesCount { get; private set; }
		public int LambdasGathered { get; private set; }
		public CheckResult State { get; private set; }

		public int Height { get; private set; }
		public int Width { get; private set; }
		private MapCell[,] map;

		private Stack<MoveLog> log = new Stack<MoveLog>();
		private SortedSet<Vector> activeRocks = new SortedSet<Vector>(new VectorComparer());

		public int TotalLambdaCount { get; private set; }
		public int InitialWater { get; private set; }
		public int Water { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int StepsToIncreaseWater { get; private set; }
		public int WaterproofLeft { get; private set; }

		private List<Vector> Beard = new List<Vector>();

		public Map(string filename)
			: this(File.ReadAllLines(filename))
		{
		}

		public Map(string[] lines)
		{
			Lines = lines;
			State = CheckResult.Nothing;

			int firstBlankLineIndex = Array.IndexOf(lines, "");
			Height = firstBlankLineIndex == -1 ? lines.Length : firstBlankLineIndex;
			Width = lines.Take(Height).Max(a => a.Length);

			map = new MapCell[Width + 2, Height + 2];

			for (int row = 0; row < Height + 2; row++)
			{
				for (int col = 0; col < Width + 2; col++)
				{
					map[col, row] = MapCell.Wall;
				}
			}

			for (int row = 1; row < Height + 1; row++)
			{
				for (int col = 1; col < Width + 1; col++)
				{
					map[col, row] = MapCell.Empty;
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
					if (map[col + 1, newY + 1] == MapCell.ClosedLift || map[col + 1, newY + 1] == MapCell.OpenedLift)
					{
						LiftX = col + 1;
						LiftY = newY + 1;
					}
					if (map[col + 1, newY + 1] == MapCell.Beard)
					{
						Beard.Add(new Vector(col + 1, newY + 1));
					}
					if (TargetsChars.Contains((char)map[col + 1, newY + 1]))
					{
						Targets[map[col + 1, newY + 1]] = new Vector(col + 1, newY + 1);
					}
					if (TrampolinesChars.Contains((char)map[col + 1, newY + 1]))
					{
						Trampolines[map[col + 1, newY + 1]] = new Vector(col + 1, newY + 1);
					}
					if (map[col + 1, newY + 1] == MapCell.Lambda)
					{
						TotalLambdaCount++;
					}
				}
			}
			InitializeVariables(lines.Skip(Height + 1).ToArray());

			Height += 2;
			Width += 2;

			InitializeActiveRocks();
		}

		private void InitializeVariables(string[] floodingSpecs)
		{
			Water = 0;
			Flooding = 0;
			Waterproof = 10;
			Growth = 25;
			Razors = 0;
			foreach (var floodingSpec in floodingSpecs.Where(line => !string.IsNullOrWhiteSpace(line)))
			{
				string[] parts = floodingSpec.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts[0] == "Water") Water = int.Parse(parts[1]);
				if (parts[0] == "Flooding") Flooding = int.Parse(parts[1]);
				if (parts[0] == "Waterproof") Waterproof = int.Parse(parts[1]);
				if (parts[0] == "Trampoline")
				{
					TrampToTarget[(MapCell) parts[1][0]] = (MapCell) parts[3][0];
				}
				if (parts[0] == "Growth") Growth = int.Parse(parts[1]);
				if (parts[0] == "Razors") Razors = int.Parse(parts[1]);
			}

			GrowthLeft = Growth;
			StepsToIncreaseWater = Flooding;
			WaterproofLeft = Waterproof;
			InitialWater = Water;
		}

		private void InitializeActiveRocks()
		{
			for (int y = 1; y < Height - 1; y++)
			{
				for (int x = 1; x < Width - 1; x++)
				{
					if (map[x, y].IsRock() && map[x, y - 1].IsRock())
					{
						activeRocks.Add(new Vector(x, y));
					}
					if (map[x, y].IsRock() && map[x, y - 1].IsRock()
						&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						activeRocks.Add(new Vector(x, y));
					}
					if (map[x, y].IsRock() && map[x, y - 1].IsRock()
						&& (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
						&& map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
					{
						activeRocks.Add(new Vector(x, y));
					}
					if (map[x, y].IsRock() && map[x, y - 1] == MapCell.Lambda
						&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
					{
						activeRocks.Add(new Vector(x, y));
					}
					if (map[x, y] == MapCell.ClosedLift && LambdasGathered == TotalLambdaCount)
					{
						map[x, y] = MapCell.OpenedLift;
					}
				}
			}
		}

		public Vector Robot { get { return new Vector(RobotX, RobotY); } }

		public int RobotX { get; private set; }
		public int RobotY { get; private set; }

		public Vector Lift { get { return new Vector(LiftX, LiftY); } }
		public int LiftX { get; private set; }
		public int LiftY { get; private set; }

		public bool HasActiveRocks
		{
			get { return activeRocks.Any(a => a != TryToMoveRock(a)); }
		}

		public MapCell this[Vector pos]
		{
			get { return map[pos.X, pos.Y]; }
			set { map[pos.X, pos.Y] = value; }
		}

		public MapCell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public override string ToString()
		{
			return new MapSerializer().Serialize(map.SkipBorder(), Water, Flooding, Waterproof, TrampToTarget);
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
				case '@':
					return MapCell.LambdaRock;
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
				case '!':
					return MapCell.Razor;
				case 'W':
					return MapCell.Beard;
				default:
					if (TrampolinesChars.Contains(c))
						return (MapCell)c;
					if (TargetsChars.Contains(c))
						return (MapCell)c;
					break;
			}

			throw new Exception("InvalidMap " + c);
		}

		public Map Move(RobotMove move)
		{
			log.Push(new MoveLog{PreviousWaterproofLeft = WaterproofLeft});

			if (move == RobotMove.Abort)
			{
				State = CheckResult.Abort;
				return this;
			}

			if (State != CheckResult.Nothing)
				throw new GameFinishedException();

			MovesCount++;
			if (move != RobotMove.Wait && move != RobotMove.CutBeard)
			{
				Vector newRobot = Robot.Add(move.ToVector());
				if (CheckValid(newRobot.X, newRobot.Y))
				{
					log.Peek().RobotMove = new Movement { PreviousX = RobotX, PreviousY = RobotY, NextX = newRobot.X, NextY = newRobot.Y };
					log.Peek().RemovedObjects.Add(Tuple.Create(newRobot, this[newRobot]));
					DoMove(newRobot.X, newRobot.Y);
				}
				else
				{
					log.Peek().RobotMove = new Movement { PreviousX = RobotX, PreviousY = RobotY, NextX = RobotX, NextY = RobotY };
				}
			}
			else
				log.Peek().RobotMove = new Movement { PreviousX = RobotX, PreviousY = RobotY, NextX = RobotX, NextY = RobotY };

			if(move == RobotMove.CutBeard)
			{
				if(!CutBeard())
					throw new Exception();
			}

			if (State != CheckResult.Win)
				Update();

			return this;
		}

		private bool CutBeard()
		{
			if (Razors == 0)
				return false;

			var list = new List<Vector>();

			foreach (var b in Beard)
			{
				if(Robot.Distance(b) > 1)
					list.Add(b);
				else
					this[b] = MapCell.Empty;
			}

			Beard = list;

			return true;
		}

		private bool CheckValid(int newRobotX, int newRobotY)
		{
			if (map[newRobotX, newRobotY] == MapCell.Wall || map[newRobotX, newRobotY].IsTarget() ||
				map[newRobotX, newRobotY] == MapCell.ClosedLift || map[newRobotX, newRobotY] == MapCell.Beard)
				return false;

			if (!map[newRobotX, newRobotY].IsRock())
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
			MapCell newMapCell = map[newRobotX, newRobotY];
			if (newMapCell == MapCell.Lambda)
			{
				LambdasGathered++;
			}
			else if (newMapCell.IsTrampoline())
			{
				var target = TrampToTarget[newMapCell];
				Vector targetCoords = Targets[target];
				newRobotX = targetCoords.X;
				newRobotY = targetCoords.Y;

				log.Peek().RemovedObjects.Add(new Tuple<Vector, MapCell>(targetCoords, target));
				foreach (KeyValuePair<MapCell, MapCell> pair in TrampToTarget.Where(a => a.Value == target))
				{
					Vector trampolinePos = Trampolines[pair.Key];
					this[trampolinePos] = MapCell.Empty;
					log.Peek().RemovedObjects.Add(new Tuple<Vector, MapCell>(trampolinePos, pair.Key));
					CheckNearRocks(activeRocks, trampolinePos.X, trampolinePos.Y);
				}
			}
			else if (newMapCell == MapCell.Earth)
			{
			}
			else if (newMapCell == MapCell.Razor)
			{
				Razors++;
			}
			else if (newMapCell == MapCell.OpenedLift)
			{
				State = CheckResult.Win;
			}
			else if (newMapCell.IsRock())
			{
				int rockX = newRobotX * 2 - RobotX;
				map[rockX, newRobotY] = map[newRobotX, newRobotY];
				log.Peek().MovingRocks.Add(
					new Movement
					{
						PreviousX = newRobotX,
						PreviousY = newRobotY,
						NextX = rockX,
						NextY = newRobotY
					});
				activeRocks.Add(new Vector(rockX, newRobotY));
			}
			map[RobotX, RobotY] = MapCell.Empty;
			if (newMapCell != MapCell.OpenedLift)
				map[newRobotX, newRobotY] = MapCell.Robot;

			CheckNearRocks(activeRocks, RobotX, RobotY);

			RobotX = newRobotX;
			RobotY = newRobotY;
		}

		private void CheckNearRocks(SortedSet<Vector> updateableRocks, int x, int y)
		{
			for (int rockX = x - 1; rockX <= x + 1; rockX++)
			{
				for (int rockY = y; rockY <= y + 1; rockY++)
				{
					var coords = new Vector(rockX, rockY);
					if (!coords.Equals(TryToMoveRock(rockX, rockY)))
						updateableRocks.Add(coords);
				}
			}
		}

		public bool IsInWater(int movesDone, int y)
		{
			return y <= WaterLevelAfterUpdate(MovesCount + movesDone);
		}

		public int WaterLevelAfterUpdate(int updateNumber)
		{
			return Flooding == 0 ? Water : updateNumber / Flooding + InitialWater;
		}

		public bool IsSafeMove(Vector from, Vector to, int movesDone, int waterproofLeft)
		{
			if (waterproofLeft <= 0 && WaterLevelAfterUpdate(MovesCount + movesDone-1) >= to.Y)
				return false;

			var swap = map[from.X, from.Y];
			map[RobotX, RobotY] = MapCell.Empty;
			map[from.X, from.Y] = MapCell.Empty;

			bool isSafe = true;

			if(to.Y == from.Y - 1)
			{
				for (int x = to.X - 1; x <= to.X + 1; x++)
				{
					var newPosition = TryToMoveRock(new Vector(x, to.Y + 2));
					if (newPosition.X == to.X && newPosition.Y == to.Y + 1)
						isSafe = false;
				}
			}

			if (to.Y + movesDone + 1 < Height)//камни сверху
			{
				int y = to.Y + movesDone + 1;
				for (int x = to.X - 1; x <= to.X + 1; x++)
				{
					var newPosition = TryToMoveRock(new Vector(x, y));

					if (newPosition.X == to.X && newPosition.Y == y - 1 && IsColumnEmpty(to.X, to.Y + 1, y - 2))
						isSafe = false;
				}
			}

			map[from.X, from.Y] = swap;
			map[RobotX, RobotY] = MapCell.Robot;

			return isSafe;
		}

		private bool IsColumnEmpty(int x, int bottomY, int topY)
		{
			for(int y = bottomY; y <= topY; y++)
			{
				if (map[x, y] != MapCell.Empty)
					return false;
			}
			return true;
		}

		private Vector TryToMoveRock(Vector coords)
		{
			return TryToMoveRock(coords.X, coords.Y);
		}

		private Vector TryToMoveRock(int x, int y)
		{
			if (map[x, y].IsRock() && map[x, y - 1] == MapCell.Empty)
			{
				return new Vector(x, y - 1);
			}
			if (map[x, y].IsRock() && map[x, y - 1].IsRock()
				&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
			{
				return new Vector(x + 1, y - 1);
			}
			if (map[x, y].IsRock() && map[x, y - 1].IsRock()
				&& (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
				&& map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
			{
				return new Vector(x - 1, y - 1);
			}
			if (map[x, y].IsRock() && map[x, y - 1] == MapCell.Lambda
				&& map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
			{
				return new Vector(x + 1, y - 1);
			}

			return new Vector(x, y);
		}

		private void Update()
		{
			var robotFailed = false;

			var newActiveRocks = new SortedSet<Vector>(new VectorComparer());
			var rockMoves = new Dictionary<Vector, Vector>();

			foreach (var activeRockCoords in activeRocks)
			{
				var newCoords = TryToMoveRock(activeRockCoords);
				if (!activeRockCoords.Equals(newCoords) && map[activeRockCoords.X, activeRockCoords.Y].IsRock())
					rockMoves.Add(activeRockCoords, newCoords);
			}

			bool killedByRock = false;
			foreach (var rockMove in rockMoves)
			{
				if (!map[rockMove.Value.X, rockMove.Value.Y].IsRock())
					newActiveRocks.Add(rockMove.Value);

				if (this[rockMove.Key] == MapCell.LambdaRock && this[rockMove.Value.Sub(new Vector(0, 1))] != MapCell.Empty)
					this[rockMove.Value] = MapCell.Lambda;
				else
					this[rockMove.Value] = this[rockMove.Key];
				map[rockMove.Key.X, rockMove.Key.Y] = MapCell.Empty;

				log.Peek().MovingRocks.Add(
					new Movement
						{
							PreviousX = rockMove.Key.X,
							PreviousY = rockMove.Key.Y,
							NextX = rockMove.Value.X,
							NextY = rockMove.Value.Y
						});
				killedByRock = IsRobotKilledByRock(rockMove.Value.X, rockMove.Value.Y);
				robotFailed |= killedByRock;
				CheckNearRocks(newActiveRocks, rockMove.Key.X, rockMove.Key.Y);
			}

			activeRocks = newActiveRocks;

			if (TotalLambdaCount == LambdasGathered)
				map[LiftX, LiftY] = MapCell.OpenedLift;

			if (RobotX == LiftX && RobotY == LiftY && map[LiftX, LiftY] == MapCell.OpenedLift)
			{
				State = CheckResult.Win;
			}

			CheckBeardGrowth();

			if (robotFailed)
			{
				State = CheckResult.Fail;
				throw new KilledByRockException();
			}

			robotFailed |= IsRobotKilledByFlood();

			if (robotFailed)
			{
				State = CheckResult.Fail;
				throw killedByRock ? new KilledByRockException() : new GameFinishedException();
			}
		}

		private bool IsRobotKilledByFlood()
		{
			if (Water >= RobotY) WaterproofLeft--;
			else WaterproofLeft = Waterproof;
			if (Flooding > 0)
			{
				StepsToIncreaseWater--;
				if (StepsToIncreaseWater == 0)
				{
					Water++;
					StepsToIncreaseWater = Flooding;
				}
			}
			return WaterproofLeft < 0;
		}

		private bool IsRobotKilledByRock(int x, int y)
		{
			return map[x, y - 1] == MapCell.Robot;
		}

		private void CheckBeardGrowth()
		{
			GrowthLeft--;
			if(GrowthLeft == 0)
			{
				GrowthLeft = Growth;

				foreach (var b in Beard.ToList())
				{
					for(int i = -1; i <= 1; i++)
					{
						for(int j = -1; j <= 1; j++)
						{
							var newVector = b.Add(new Vector(i, j));
							if(this[newVector] == MapCell.Empty)
							{
								this[newVector] = MapCell.Beard;
								Beard.Add(newVector);
							}
						}
					}
				}
			}
		}

		public bool Rollback()
		{
			if (MovesCount == 0)
				return false;

			if (log.Count == 0) return false;
			var stateLog = log.Pop();

			if (State == CheckResult.Abort)
			{
				State = CheckResult.Nothing;
				return true;
			}
			if (Flooding > 0)
			{
				StepsToIncreaseWater++;
				if(StepsToIncreaseWater == Flooding + 1)
				{
					StepsToIncreaseWater -= Flooding;
					Water--;
				}
			}

			MovesCount--;

			WaterproofLeft = stateLog.PreviousWaterproofLeft;
	
			stateLog.MovingRocks.Reverse();

			foreach (var rock in stateLog.MovingRocks)
			{
				map[rock.PreviousX, rock.PreviousY] = MapCell.Rock;
				map[rock.NextX, rock.NextY] = MapCell.Empty;
			}

			if(stateLog.RobotMove != null)
			{
				RobotX = stateLog.RobotMove.PreviousX;
				RobotY = stateLog.RobotMove.PreviousY;
			}
			map[RobotX, RobotY] = MapCell.Robot;

			foreach (Tuple<Vector, MapCell> removedObj in stateLog.RemovedObjects)
			{
				this[removedObj.Item1] = removedObj.Item2;

				if (removedObj.Item2 == MapCell.Lambda)
				{
					if (LambdasGathered == TotalLambdaCount) map[LiftX, LiftY] = MapCell.ClosedLift;
					LambdasGathered--;
				}
			}

			activeRocks = new SortedSet<Vector>(new VectorComparer());
			stateLog.MovingRocks.ForEach(a => activeRocks.Add(new Vector(a.PreviousX, a.PreviousY)));

			State = CheckResult.Nothing;
			return true;
		}
	}

	public class Movement
	{
		public int PreviousX;
		public int PreviousY;
		public int NextX;
		public int NextY;
	}

	public class MoveLog
	{
		public Movement RobotMove;
		public List<Tuple<Vector, MapCell>> RemovedObjects = new List<Tuple<Vector, MapCell>>();
		public List<Movement> MovingRocks = new List<Movement>();
		public int PreviousWaterproofLeft;
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