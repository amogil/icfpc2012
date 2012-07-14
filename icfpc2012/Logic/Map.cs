using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

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
		Fail,
		Abort,
	}

	public interface IMap
	{
		int MovesCount { get; }
		int LambdasGathered { get; }
		CheckResult State { get; }
		int TotalLambdaCount { get; }
		int Water { get; }
		int Flooding { get; }
		int Waterproof { get; }
		int StepsToIncreaseWater { get; }
		int WaterproofLeft { get; }
		int Height { get; }
		int Width { get; }
		string GetMapStateAsAscii();
		IMap Move(RobotMove move);
		bool IsSafeMove(Vector from, Vector to);
		bool LoadPreviousState();
		MapCell this[Vector pos] { get; }
		MapCell this[int x, int y] { get; }
		Vector Robot { get; }
	}

	public class Map : IMap
	{
		public int MovesCount { get; private set; }
		public int LambdasGathered { get; private set; }
		public CheckResult State { get; private set; }

		public int Height { get; private set; }
		public int Width { get; private set; }
		private MapCell[,] map;

        private Stack<MoveLog> log = new Stack<MoveLog>();
		private HashSet<Vector> activeRocks = new HashSet<Vector>();

		public int TotalLambdaCount { get; private set; }
		public int Water { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int StepsToIncreaseWater { get; private set; }
		public int WaterproofLeft { get; private set; }

		public Map(string filename)
			:this(File.ReadAllLines(filename))
		{
		}

		public Map(string[] lines)
		{
			State = CheckResult.Nothing;

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

			Height += 2;
			Width += 2;

		    InitializeActiveRocks();
		}

		private void InitializeFlooding(string[] floodingSpecs)
		{
			Water = 0;
			Flooding = 0;
			Waterproof = 10;
			foreach (var floodingSpec in floodingSpecs.Where(line => !string.IsNullOrWhiteSpace(line)))
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
                        activeRocks.Add(new Vector(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                        && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
                    {
						activeRocks.Add(new Vector(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                        && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
                        && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
                    {
						activeRocks.Add(new Vector(x, y));
                    }
                    if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
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

		public Vector Robot {get {return new Vector(RobotX, RobotY);}}
	    public int RobotX { get; private set; }
		public int RobotY { get; private set; }

		public Vector Lift { get { return new Vector(LiftX, LiftY); } }
        public int LiftX { get; private set; }
        public int LiftY { get; private set; }
    
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
            log.Push(new MoveLog());

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

				if (CheckValid(newRobotX, newRobotY))
				{

					log.Peek().RobotMove = new Movement {PreviousX = RobotX, PreviousY = RobotY, NextX = newRobotX, NextY = newRobotY};
					log.Peek().EatedObject = map[newRobotX, newRobotY];
			    	DoMove(newRobotX, newRobotY);
				}
				else
				{
					log.Peek().RobotMove = new Movement { PreviousX = RobotX, PreviousY = RobotY, NextX = RobotX, NextY = RobotY };
					log.Peek().EatedObject = this[Robot];
				}
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
            map[newRobotX, newRobotY] = MapCell.Robot;

            CheckNearRocks(activeRocks, RobotX, RobotY);

			RobotX = newRobotX;
			RobotY = newRobotY;
		}

        private void CheckNearRocks(HashSet<Vector> updateableRocks, int x, int y)
        {
            for(int rockX = x - 1; rockX <= x + 1; rockX ++)
            {
                for(int rockY = y; rockY <= y + 1; rockY++)
                {
					var coords = new Vector(rockX, rockY);
                    if (!coords.Equals(TryToMoveRock(rockX, rockY)))
                        updateableRocks.Add(coords);
                }
            }
        }

		public bool IsSafeMove(Vector from, Vector to)
		{
			if (to.Y + 2 > Height)
				return true;

			bool isSafe = true;

			var swap = map[from.X, from.Y];
			map[RobotX, RobotY] = MapCell.Empty;
			map[from.X, from.Y] = MapCell.Empty;

			int y = to.Y + 2;
			for (int x = to.X - 1; x <= to.X + 1; x++)
			{
				var newPosition = TryToMoveRock(new Vector(x, y));
				if (newPosition.X == to.X && newPosition.Y == to.Y + 1)
					isSafe = false;
			}

			map[from.X, from.Y] = swap;
			map[RobotX, RobotY] = MapCell.Robot;

			return isSafe;
		}

		private Vector TryToMoveRock(Vector coords)
        {
            return TryToMoveRock(coords.X, coords.Y);
        }

		private Vector TryToMoveRock(int x, int y)
        {
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Empty)
            {
				return new Vector(x, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
            {
				return new Vector(x + 1, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock
                && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
                && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
            {
				return new Vector(x - 1, y - 1);
            }
            if (map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Lambda
                && map[x + 1, y] == MapCell.Empty && map[x + 1, y - 1] == MapCell.Empty)
            {
				return new Vector(x + 1, y - 1);
            }

			return new Vector(x, y);
        }

	    private void Update()
	    {
			var newActiveRocks = new HashSet<Vector>();
			var rockMoves = new Dictionary<Vector, Vector>();

	        foreach (var activeRockCoords in activeRocks)
	        {
                var newCoords = TryToMoveRock(activeRockCoords);
                if (!activeRockCoords.Equals(newCoords) && map[activeRockCoords.X, activeRockCoords.Y] == MapCell.Rock)
                    rockMoves.Add(activeRockCoords, newCoords);
	        }

	        foreach (var rockMove in rockMoves)
            {
                map[rockMove.Key.X, rockMove.Key.Y] = MapCell.Empty;
                if (map[rockMove.Value.X, rockMove.Value.Y] != MapCell.Rock)
                    newActiveRocks.Add(rockMove.Value);
                map[rockMove.Value.X, rockMove.Value.Y] = MapCell.Rock;
                log.Peek().MovingRocks.Add(
                    new Movement
                        {
                            PreviousX = rockMove.Key.X, 
                            PreviousY = rockMove.Key.Y, 
                            NextX = rockMove.Value.X,
                            NextY = rockMove.Value.Y
                        });
				CheckRobotDanger(rockMove.Value.X, rockMove.Value.Y);
				CheckNearRocks(newActiveRocks, rockMove.Key.X, rockMove.Key.Y);
	        }

	        activeRocks = newActiveRocks;

            if(TotalLambdaCount == LambdasGathered)
                map[LiftX, LiftY] = MapCell.OpenedLift;

            if (RobotX == LiftX && RobotY == LiftY && map[LiftX, LiftY] == MapCell.OpenedLift)
            {
                State = CheckResult.Win;
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
				throw new GameFinishedException();
			}
		}

        public bool LoadPreviousState()
        {
			if (log.Count == 0) return false;
        	MovesCount--;

            var stateLog = log.Pop();
        	stateLog.MovingRocks.Reverse();

            foreach (var rock in stateLog.MovingRocks)
            {
                map[rock.PreviousX, rock.PreviousY] = MapCell.Rock;
                map[rock.NextX, rock.NextY] = MapCell.Empty;
            }

            RobotX = stateLog.RobotMove.PreviousX;
            RobotY = stateLog.RobotMove.PreviousY;
            map[RobotX, RobotY] = MapCell.Robot;
            map[stateLog.RobotMove.NextX, stateLog.RobotMove.NextY] = stateLog.EatedObject;

        	if (stateLog.EatedObject == MapCell.Lambda)
        	{
        		if (LambdasGathered == TotalLambdaCount) map[LiftX, LiftY] = MapCell.ClosedLift;
        		LambdasGathered--;
        	}

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
        public MapCell EatedObject;
        public List<Movement> MovingRocks = new List<Movement>();
    }

    public class NoMoveException : Exception
	{
	}

	public class GameFinishedException : Exception
	{
	}

	[TestFixture]
	public class MapIsSafeMove_Test
	{
		[Test]
		public void Test()
		{
			Map contest1 = WellKnownMaps.Contest1();
			Map contest4 = WellKnownMaps.Contest4();
			Assert.IsTrue(contest1.IsSafeMove(new Vector(5, 5), new Vector(5, 4)));
			Assert.IsFalse(contest1.IsSafeMove(new Vector(5, 4), new Vector(5, 3)));
			Assert.IsFalse(contest4.IsSafeMove(new Vector(3, 7), new Vector(3, 6)));
			Assert.IsTrue(contest4.IsSafeMove(new Vector(3, 6), new Vector(3, 5)));
		}
	}
}