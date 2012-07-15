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

	public static class MapCellExtension
	{
		public static bool CanStepUp(this MapCell cell)
		{
			return
				cell == MapCell.Empty ||
				cell == MapCell.Earth ||
				cell == MapCell.Lambda ||
				cell == MapCell.Trampoline1 ||
				cell == MapCell.Trampoline2 ||
				cell == MapCell.Trampoline3 ||
				cell == MapCell.Trampoline4 ||
				cell == MapCell.Trampoline5 ||
				cell == MapCell.Trampoline6 ||
				cell == MapCell.Trampoline7 ||
				cell == MapCell.Trampoline8 ||
				cell == MapCell.Trampoline9;
		}
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

		private SortedSet<Vector> activeRocks = new SortedSet<Vector>(new VectorComparer());

		public int TotalLambdaCount { get; private set; }
		public int InitialWater { get; private set; }
		public int Water { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int StepsToIncreaseWater { get; private set; }
		public int WaterproofLeft { get; private set; }

		private List<Vector> Beard = new List<Vector>();

		private Map()
		{
		}

		public Map(string filename)
			: this(File.ReadAllLines(filename))
		{
		}

		public Map(string[] lines)
		{
			Lines = lines;
			State = CheckResult.Nothing;

			var firstBlankLineIndex = Array.IndexOf(lines, "");
			Height = firstBlankLineIndex == -1 ? lines.Length : firstBlankLineIndex;
			Width = lines.Take(Height).Max(a => a.Length);

			map = new MapCell[Width + 2, Height + 2];
			for (var row = 0; row < Height + 2; row++)
				for (var col = 0; col < Width + 2; col++)
				{
					if (row == 0 || row == Height + 1 || col == 0 || col == Width + 1)
						SetCell(col, row, MapCell.Wall);
					else
					{
						var x = col - 1;
						var y = Height - row;
						var line = lines[y].PadRight(Width, ' ');
						var mapCell = Parse(line[x]);
						SetCell(col, row, mapCell);
						if (mapCell == MapCell.Lambda)
						{
							TotalLambdaCount++;
						}
						else if (mapCell == MapCell.Robot)
						{
							RobotX = col;
							RobotY = row;
						}
						else if (mapCell == MapCell.ClosedLift || mapCell == MapCell.OpenedLift)
						{
							LiftX = col;
							LiftY = row;
						}
						if (mapCell == MapCell.Beard)
						{
							Beard.Add(new Vector(col, row));
						}
						else if (TargetsChars.Contains((char)mapCell))
						{
							Targets[mapCell] = new Vector(col, row);
						}
						if (TrampolinesChars.Contains((char)mapCell))
						{
							Trampolines[mapCell] = new Vector(col, row);
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
					TrampToTarget[(MapCell)parts[1][0]] = (MapCell)parts[3][0];
				}
				if (parts[0] == "Growth") Growth = int.Parse(parts[1]);
				if (parts[0] == "Razors") Razors = int.Parse(parts[1]);
			}

			GrowthLeft = Growth;
			StepsToIncreaseWater = Flooding;
			WaterproofLeft = Waterproof;
			InitialWater = Water;
		}

		public MapCell GetCell(int x, int y)
		{
			return map[x, y];
		}

		public MapCell GetCell(Vector pos)
		{
			return GetCell(pos.X, pos.Y);
		}

		private MapCell SetCell(Vector pos, MapCell newValue)
		{
			return SetCell(pos.X, pos.Y, newValue);
		}

		private MapCell SetCell(int x, int y, MapCell newValue)
		{
			var oldValue = map[x, y];
			map[x, y] = newValue;
			return oldValue;
		}

		private void InitializeActiveRocks()
		{
			for (var y = 1; y < Height - 1; y++)
				for (var x = 1; x < Width - 1; x++)
				{
					var pos = new Vector(x, y);
					var newRockPos = TryToMoveRock(pos, this);
					if (!pos.Equals(newRockPos))
						activeRocks.Add(pos);
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
			get { return activeRocks.Any(a => a != TryToMoveRock(a, this)); }
		}

		public override string ToString()
		{
			return new MapSerializer().Serialize(this.SkipBorder(), Water, Flooding, Waterproof, TrampToTarget);
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

		private Map Clone()
		{
			var clone = new Map
			{
				Width = Width,
				Height = Height,
				map = new MapCell[Width,Height],
				activeRocks = new HashSet<Vector>(),
				Flooding = Flooding,
				LambdasGathered = LambdasGathered,
				LiftX = LiftX,
				LiftY = LiftY,
				MovesCount = MovesCount,
				RobotX = RobotX,
				RobotY = RobotY,
				State = State,
				InitialWater = InitialWater,
				StepsToIncreaseWater = StepsToIncreaseWater,
				Targets = new Dictionary<MapCell, Vector>(Targets.Select(kvp => new {kvp.Key, kvp.Value}).ToDictionary(kvp => kvp.Key, kvp=>kvp.Value)),
				TotalLambdaCount = TotalLambdaCount,
				TrampToTarget = new Dictionary<MapCell, MapCell>(TrampToTarget.Select(kvp => new {kvp.Key, kvp.Value}).ToDictionary(t => t.Key, t => t.Value)),
				Trampolines = new Dictionary<MapCell, Vector>(Trampolines.Select(kvp => new {kvp.Key, kvp.Value}).ToDictionary(kvp => kvp.Key, kvp=>kvp.Value)),
				Water = Water,
				Waterproof = Waterproof,
				WaterproofLeft = WaterproofLeft,
			};
			for (var row = 0; row < Height; row++)
				for (var col = 0; col < Width; col++)
					clone.SetCell(col, row, GetCell(col, row));
			return clone;
		}

		public Map Move(RobotMove move)
		{
			var newMap = Clone();

			if (move == RobotMove.Abort)
			{
				newMap.State = CheckResult.Abort;
				return newMap;
			}

			newMap.MovesCount++;
			if (move != RobotMove.Wait && move != RobotMove.CutBeard)
			{
				var newRobot = Robot.Add(move.ToVector());
				if (CheckValid(newRobot.X, newRobot.Y))
					DoMove(newRobot.X, newRobot.Y, newMap);
			}

			if(move == RobotMove.CutBeard)
			{
				if(!CutBeard())
					throw new Exception();
			}

			if (newMap.State != CheckResult.Win)
				Update();

			return newMap;
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

			if (GetCell(checkX, RobotY) == MapCell.Empty)
				return true;

			return false;
		}

		private void DoMove(int newRobotX, int newRobotY, Map newMap)
		{
			var newMapCell = GetCell(newRobotX, newRobotY);
			if (newMapCell == MapCell.Lambda)
			{
				newMap.LambdasGathered++;
			}
			else if (newMapCell.IsTrampoline())
			{
				var target = TrampToTarget[newMapCell];
				var targetCoords = Targets[target];
				newRobotX = targetCoords.X;
				newRobotY = targetCoords.Y;

				foreach (var pair in TrampToTarget.Where(a => a.Value == target))
				{
					var trampolinePos = Trampolines[pair.Key];
					newMap.SetCell(trampolinePos, MapCell.Empty);
					CheckNearRocks(newMap.activeRocks, trampolinePos.X, trampolinePos.Y, newMap);
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
				newMap.State = CheckResult.Win;
			}
			else if (newMapCell.IsRock())
			{
				var rockX = newRobotX * 2 - RobotX;
				newMap.SetCell(rockX, newRobotY, map[newRobotX, newRobotY]);
				newMap.activeRocks.Add(new Vector(rockX, newRobotY));
			}
			newMap.SetCell(RobotX, RobotY, MapCell.Empty);
			if (newMapCell != MapCell.OpenedLift)
				newMap.SetCell(newRobotX, newRobotY, MapCell.Robot);

			CheckNearRocks(newMap.activeRocks, RobotX, RobotY, newMap);

			newMap.RobotX = newRobotX;
			newMap.RobotY = newRobotY;
		}

		private void CheckNearRocks(SortedSet<Vector> updateableRocks, int x, int y, Map mapToUse)
		{
			for (int rockX = x - 1; rockX <= x + 1; rockX++)
			{
				for (int rockY = y; rockY <= y + 1; rockY++)
				{
					var coords = new Vector(rockX, rockY);
					if (!coords.Equals(TryToMoveRock(rockX, rockY, mapToUse)))
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

		private static Vector TryToMoveRock(Vector coords, Map mapToUse)
		{
			return TryToMoveRock(coords.X, coords.Y, mapToUse);
		}

		private static Vector TryToMoveRock(int x, int y, Map mapToUse)
		{
			if (mapToUse.GetCell(x, y) == MapCell.Rock && mapToUse.GetCell(x, y - 1) == MapCell.Empty)
			{
				return new Vector(x, y - 1);
			}
			if (mapToUse.GetCell(x, y) == MapCell.Rock && mapToUse.GetCell(x, y - 1) == MapCell.Rock
				&& mapToUse.GetCell(x + 1, y) == MapCell.Empty && mapToUse.GetCell(x + 1, y - 1) == MapCell.Empty)
			{
				return new Vector(x + 1, y - 1);
			}
			if (mapToUse.GetCell(x, y) == MapCell.Rock && mapToUse.GetCell(x, y - 1) == MapCell.Rock
				&& (mapToUse.GetCell(x + 1, y) != MapCell.Empty || mapToUse.GetCell(x + 1, y - 1) != MapCell.Empty)
				&& mapToUse.GetCell(x - 1, y) == MapCell.Empty && mapToUse.GetCell(x - 1, y - 1) == MapCell.Empty)
			{
				return new Vector(x - 1, y - 1);
			}
			if (mapToUse.GetCell(x, y) == MapCell.Rock && mapToUse.GetCell(x, y - 1) == MapCell.Lambda
				&& mapToUse.GetCell(x + 1, y) == MapCell.Empty && mapToUse.GetCell(x + 1, y - 1) == MapCell.Empty)
			{
				return new Vector(x + 1, y - 1);
			}

			return new Vector(x, y);
		}

		private void Update(Map newMap)
		{
			var robotFailed = false;

			var newActiveRocks = new SortedSet<Vector>(new VectorComparer());
			var rockMoves = new Dictionary<Vector, Vector>();
			foreach (var activeRockCoords in activeRocks.Concat(newMap.activeRocks).Distinct())
			{
				var newCoords = TryToMoveRock(activeRockCoords, newMap);
				if (!activeRockCoords.Equals(newCoords) && GetCell(activeRockCoords.X, activeRockCoords.Y).IsRock())
					rockMoves.Add(activeRockCoords, newCoords);
			}
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
				CheckNearRocks(newActiveRocks, rockMove.Key.X, rockMove.Key.Y, newMap);
			}
			newMap.activeRocks = newActiveRocks;

			if (newMap.TotalLambdaCount == newMap.LambdasGathered)
				newMap.SetCell(LiftX, LiftY, MapCell.OpenedLift);

			CheckBeardGrowth();

			if (robotFailed)
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

		private static bool IsRobotKilledByFlood(Map newMap)
		{
			if (newMap.Water >= newMap.RobotY) newMap.WaterproofLeft--;
			else newMap.WaterproofLeft = newMap.Waterproof;
			if (newMap.Flooding > 0)
			{
				newMap.StepsToIncreaseWater--;
				if (newMap.StepsToIncreaseWater == 0)
				{
					newMap.Water++;
					newMap.StepsToIncreaseWater = newMap.Flooding;
				}
			}
			return newMap.WaterproofLeft < 0;
		}

		private static bool IsRobotKilledByRock(int x, int y, Map mapToUse)
		{
			return mapToUse.GetCell(x, y - 1) == MapCell.Robot;
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
				SetCell(rock.PreviousX, rock.PreviousY, MapCell.Rock);
				SetCell(rock.NextX, rock.NextY, MapCell.Empty);
			}

			if(stateLog.RobotMove != null)
			{
				RobotX = stateLog.RobotMove.PreviousX;
				RobotY = stateLog.RobotMove.PreviousY;
			}
			SetCell(RobotX, RobotY, MapCell.Robot);

			foreach (Tuple<Vector, MapCell> removedObj in stateLog.RemovedObjects)
			{
				SetCell(removedObj.Item1, removedObj.Item2);

				if (removedObj.Item2 == MapCell.Lambda)
				{
					if (LambdasGathered == TotalLambdaCount) SetCell(LiftX, LiftY, MapCell.ClosedLift);
					LambdasGathered--;
				}
			}

			activeRocks = new SortedSet<Vector>(new VectorComparer());
			stateLog.MovingRocks.ForEach(a => activeRocks.Add(new Vector(a.PreviousX, a.PreviousY)));

			State = CheckResult.Nothing;
			return true;
		}
	}

	public class GameFinishedException : Exception
	{
	}
}