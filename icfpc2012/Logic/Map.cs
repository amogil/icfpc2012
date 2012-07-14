using System;
using System.Collections.Generic;
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
		public int Score { get; private set; }
		public int LambdasGathered { get; private set; }

		public readonly int Height;
		public readonly int Width;
		public CheckResult State = CheckResult.Nothing;
		private MapCell[,] map;

        private Stack<MoveLog> log = new Stack<MoveLog>();
        private HashSet<Tuple<int, int>> activeRocks = new HashSet<Tuple<int, int>>();

		public int TotalLambdaCount { get; private set; }
		public int Water { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int StepsToIncreaseWater { get; private set; }
		public int WaterproofLeft { get; private set; }

		public Map(string[] lines)
		{
			int firstBlankLineIndex = Array.IndexOf(lines, "");
			Height = firstBlankLineIndex == -1 ? lines.Length : firstBlankLineIndex;
			Width = lines.Max(a => a.Length);

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
				    if (map[col + 1, newY + 1] == MapCell.Lambda)
					{
						TotalLambdaCount++;
					}
				}
			}
			InitializeFlooding(lines.Skip(Height + 1).ToArray());
		    InitializeActiveRocks();

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
        
        private void InitializeActiveRocks()
        {
            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Empty)
                    {
                        activeRocks.Add(new Tuple<int, int>(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                        && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
                    {
                        activeRocks.Add(new Tuple<int, int>(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                        && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
                        && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
                    {
                        activeRocks.Add(new Tuple<int, int>(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
                        && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
                    {
                        activeRocks.Add(new Tuple<int, int>(x, y));
                    }
                    if (map[x, y] == MapCell.ClosedLift && LambdasGathered == TotalLambdaCount)
                    {
                        map[x, y] = MapCell.OpenedLift;
                    }
                }
            }
        }


	    public int RobotX { get; private set; }
		public int RobotY { get; private set; }

        public int LiftX { get; private set; }
        public int LiftY { get; private set; }

		public MapCell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public override string ToString()
		{
			return new MapSerializer().Serialize(map.SkipBorder(), Water, Flooding, Waterproof);
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
            log.Push(new MoveLog());

		    try
		    {
                if (move == RobotMove.Abort)
			    {
				    Score += LambdasGathered * 25;
				    State = CheckResult.Win;
				    return this;
			    }

			    if (State != CheckResult.Nothing)
				    throw new GameFinishedException();
            
			    Score -= 1;
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

                    log.Peek().RobotMove = new Action{PreviousX = RobotX, PreviousY = RobotY, NextX = newRobotX, NextY = newRobotY};
			        log.Peek().EatedObject = map[newRobotX, newRobotY];

				    DoMove(newRobotX, newRobotY);
			    }
			    Update();

                return this;
            }
            catch (Exception)
            {
                Score += 1;
                log.Pop();
                throw;
            }
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
				Score += 25;
				LambdasGathered++;
			}
			else if (map[newRobotX, newRobotY] == MapCell.Earth)
			{
			}
			else if (map[newRobotX, newRobotY] == MapCell.OpenedLift)
			{
				Score += LambdasGathered * 50;
				State = CheckResult.Win;
			}
			else if (map[newRobotX, newRobotY] == MapCell.Rock)
			{
				int rockX = newRobotX * 2 - RobotX;
				map[rockX, newRobotY] = MapCell.Rock;
			    activeRocks.Add(new Tuple<int, int>(rockX, newRobotY));
			}
			map[RobotX, RobotY] = MapCell.Empty;
            map[newRobotX, newRobotY] = MapCell.Robot;

            CheckNearRocks(RobotX, RobotY);

			RobotX = newRobotX;
			RobotY = newRobotY;
		}

        private void CheckNearRocks(int x, int y)
        {
            for(int rockX = x - 1; rockX <= x + 1; rockX ++)
            {
                for(int rockY = y; rockY <= y + 1; rockY++)
                {
                    var coords = new Tuple<int, int>(rockX, rockY);
                    if (!coords.Equals(TryToMoveRock(rockX, rockY)))
                        activeRocks.Add(coords);
                }
            }
        }

	    private Tuple<int, int> TryToMoveRock(Tuple<int, int> coords)
        {
            return TryToMoveRock(coords.Item1, coords.Item2);
        }

	    private Tuple<int, int> TryToMoveRock(int x, int y)
        {
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Empty)
            {
                return new Tuple<int, int>(x, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
            {
                return new Tuple<int, int>(x + 1, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
                && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
            {
                return new Tuple<int, int>(x - 1, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
                && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
            {
                return new Tuple<int, int>(x + 1, y - 1);
            }

            return new Tuple<int, int>(x, y);
        }

	    private void Update()
	    {
	        var newActiveRocks = new HashSet<Tuple<int, int>>();
	        var rockMoves = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

	        foreach (var activeRockCoords in activeRocks)
	        {
                var newCoords = TryToMoveRock(activeRockCoords);
                if (!activeRockCoords.Equals(newCoords))
                    rockMoves.Add(activeRockCoords, newCoords);
	        }

	        foreach (var rockMove in rockMoves)
            {
                map[rockMove.Key.Item1, rockMove.Key.Item2] = MapCell.Empty;
                if (map[rockMove.Value.Item1, rockMove.Value.Item2] != MapCell.Rock)
                    newActiveRocks.Add(rockMove.Value);
                map[rockMove.Value.Item1, rockMove.Value.Item2] = MapCell.Rock;
                log.Peek().FallingRocks.Add(
                    new Action
                        {
                            PreviousX = rockMove.Key.Item1, 
                            PreviousY = rockMove.Key.Item2, 
                            NextX = rockMove.Value.Item1,
                            NextY = rockMove.Value.Item2
                        });

                CheckRobotDanger(rockMove.Value.Item1, rockMove.Value.Item2);
	        }

	        activeRocks = newActiveRocks;

            if(TotalLambdaCount == LambdasGathered)
                map[LiftX, LiftY] = MapCell.OpenedLift;

            if (RobotX == LiftX && RobotY == LiftY && map[LiftX, LiftY] == MapCell.OpenedLift)
            {
                State = CheckResult.Win;
                Score += 50*LambdasGathered;
            }

			CheckWeatherConditions();
		}

		private void CheckWeatherConditions()
		{
			if (Water >= RobotY) WaterproofLeft--;
			else WaterproofLeft = Waterproof;
			if (WaterproofLeft <= 0) throw new GameFinishedException(); //утонули
			if (Flooding > 0)
			{
				StepsToIncreaseWater--;
				if (StepsToIncreaseWater == 0)
				{
					Water++;
					StepsToIncreaseWater = Flooding;
				}
			}

		}

		private void CheckRobotDanger(int x, int y)
		{
			if (map[x, y - 1] == MapCell.Robot)
			{
				State = CheckResult.Fail;
				throw new KilledByRockException();
			}
		}

        public void LoadPreviousState()
        {
            Score += 1;

            var stateLog = log.Pop();

            foreach (var rock in stateLog.FallingRocks)
            {
                map[rock.PreviousX, rock.PreviousY] = MapCell.Rock;
                map[rock.NextX, rock.NextY] = MapCell.Empty;
            }

            RobotX = stateLog.RobotMove.PreviousX;
            RobotY = stateLog.RobotMove.PreviousY;
            map[RobotX, RobotY] = MapCell.Robot;
            map[stateLog.RobotMove.NextX, stateLog.RobotMove.NextY] = stateLog.EatedObject;

            switch (stateLog.EatedObject)
            {
                case MapCell.OpenedLift:
                    Score -= 50 * LambdasGathered;
                    break;
                case MapCell.Lambda:
                    Score -= 25;
                    if(LambdasGathered == TotalLambdaCount) map[LiftX, LiftY] = MapCell.ClosedLift;
                    LambdasGathered--;
                    break;
                default:
                    if (State == CheckResult.Win) Score -= 25*LambdasGathered;
                    break;
            }

            State = CheckResult.Nothing;
        }
	}

    public class Action
    {
        public int PreviousX;
        public int PreviousY;
        public int NextX;
        public int NextY;
    }

    public class MoveLog
    {
        public Action RobotMove;
        public MapCell EatedObject;
        public List<Action> FallingRocks = new List<Action>();
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