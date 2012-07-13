using System;
using System.Linq;
using System.Collections.Generic;

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
        private CheckResult state = CheckResult.Nothing; 

        readonly public int Width;
        readonly public int Height;
        private MapCell[,] map;
	    private MapCell[,] swapMap;

        public int RobotX { get; private set; }
        public int RobotY { get; private set; }

	    private int totalLambdaCount;
	    private int lambdaCounter;

		public Map(string[] lines)
		{
		    lambdaCounter = 0;

		    Height = lines.Length;
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
                for (int col = 0; col < Width; col++ )
                {
                    var newY = Height - row - 1;

                    map[col + 1, newY + 1] = Parse(lines[row][col]);
                    if (map[col + 1, newY + 1] == MapCell.Robot)
                    {
                        RobotX = col + 1;
                        RobotY = row + 1;
                    }
                    if (map[col + 1, newY + 1] == MapCell.Lambda)
                        lambdaCounter++;
                }
            }

		    Height += 2;
		    Width += 2;

		    totalLambdaCount = lambdaCounter;
		}

        private static MapCell Parse(char c)
        {
            switch (c)
            {
                case '#': return MapCell.Wall;
                case '*': return MapCell.Rock;
                case '/': return MapCell.Lambda;
                case '.': return MapCell.Earth;
                case ' ': return MapCell.Empty;
                case 'L': return MapCell.ClosedLift;
                case 'O': return MapCell.OpenedLift;
                case 'R': return MapCell.Robot;
            }

            throw new Exception("InvalidMap");
        }

	    public MapCell this[int x, int y]
		{
			get { return map[x + 1, y + 1]; }
		}

        public Map Move(RobotMove move)
        {
            if(move == RobotMove.Abort)
            {
                state = CheckResult.Win;
                return this;
            }

            if(state != CheckResult.Nothing)
                throw new Exception("We are done!");

            if(move != RobotMove.Wait)
            {
                int newRobotX = RobotX;
                int newRobotY = RobotY;

                if (move == RobotMove.Up) newRobotY++;
                if (move == RobotMove.Down) newRobotY--;
                if (move == RobotMove.Left) newRobotX--;
                if (move == RobotMove.Right) newRobotX++;

                if(!CheckValid(newRobotX, newRobotY))
                    throw new Exception("Move is Invalid");

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

            int checkX = newRobotX * 2 - RobotX;

            if (map[checkX, RobotY] == MapCell.Empty)
                return true;

            return false;
        }

        private void DoMove(int newRobotX, int newRobotY)
        {
            if (map[newRobotX, newRobotY] == MapCell.Lambda)
                lambdaCounter--;
            else if(map[newRobotX, newRobotY] == MapCell.Earth)
            {}
            else if (map[newRobotX, newRobotY] == MapCell.OpenedLift)
            {
                state = CheckResult.Win;
            }
            else if(map[newRobotX, newRobotY] == MapCell.Rock)
            {
                int rockX = newRobotX * 2 - RobotX;
                map[rockX, newRobotY] = MapCell.Rock;
            }
            map[RobotX, RobotY] = MapCell.Empty;
            map[newRobotX, newRobotY] = MapCell.Robot;

            RobotX = newRobotX;
            RobotY = newRobotY;
        }

	    private void Update()
        {
            for (int y = 1; y < Height - 1; y++ )
            {
                for (int x = 1; x < Width - 1; y++ )
                {
                    swapMap[x, y] = map[x, y];

                    if(map[x, y] == MapCell.Rock && map[x,y-1] == MapCell.Empty)
                    {
                        swapMap[x, y] = MapCell.Empty;
                        swapMap[x,y-1] = MapCell.Rock;
                        if(map[x,y-2] == MapCell.Robot)
                            throw new Exception("We are killed by rock");
                    }
                    if(map[x, y] == MapCell.Rock && map[x,y-1] == MapCell.Rock 
                        && map[x + 1, y] == MapCell.Empty && map[x+1,y-1] == MapCell.Empty)
                    {
                        swapMap[x, y] = MapCell.Empty;
                        swapMap[x + 1, y] = MapCell.Empty;
                        swapMap[x + 1, y - 1] = MapCell.Rock;
                    }
                    if(map[x, y] == MapCell.Rock && map[x, y - 1] == MapCell.Rock 
                        && (map[x + 1, y] != MapCell.Empty || map[x + 1, y - 1] != MapCell.Empty)
                        && map[x - 1, y] == MapCell.Empty && map[x - 1, y - 1] == MapCell.Empty)
                    {
                        swapMap[x, y] = MapCell.Empty;
                        swapMap[x - 1, y] = MapCell.Empty;
                        swapMap[x, y - 1] = MapCell.Rock;
                        swapMap[x - 1, y - 1] = MapCell.Rock;
                    }
                    if(map[x,y] == MapCell.Rock && map[x,y-1] == MapCell.Lambda
                        && map[x + 1, y] == MapCell.Empty && map[x+1,y-1] == MapCell.Empty)
                    {
                        swapMap[x, y] = MapCell.Empty;
                        swapMap[x + 1, y] = MapCell.Empty;
                        swapMap[x + 1, y - 1] = MapCell.Rock;
                    }
                    if(map[x,y] == MapCell.ClosedLift && lambdaCounter == 0)
                    {
                        swapMap[x,y] = MapCell.OpenedLift;
                    }
                }
            }

	        var swap = swapMap;
	        swapMap = map;
	        map = swap;
        }
        /*
        public string Serialize()
        {
            
        }*/
    }
}