using System;

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

    public enum MoveResult
    {
        Valid,
        Invalid
    }

    public enum CheckResult
    {
        Nothing,
        Win,
        Fail
    }

	public class Map
    {
        readonly public int Width;
        readonly public int Height;
        private MapCell[,] CurrentMap;

        public int RobotX { get; private set; }
        public int RobotY { get; private set; }

		public Map(string[] lines)
		{
		    Height = lines.Length;
		    Width = lines[0].Length;

            for (int i = 0; i < Width; i++ )
            {
                for (int j = 0; j < Height; j++)
                {
                    CurrentMap[i, j] = Parse(lines[i][j]);
                    if (CurrentMap[i, j] == MapCell.Robot)
                    {
                        RobotX = i;
                        RobotY = j;
                    }
                }
            }
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
			get { return CurrentMap[x, y]; }
		}

        public MoveResult Move(RobotMove move)
        {
            throw new NotImplementedException();
        }

        public CheckResult Check()
        {
            throw new NotImplementedException();
        }

        public Map Update()
        {
            throw new NotImplementedException();
        }
	}
}