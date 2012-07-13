using System;

namespace Logic
{
	public enum RobotMove
	{
		Left,
		Right,
		Up,
		Down,
		Wait
	}
	
	public enum MapCell
	{
		Empty,
		Earth,
		Rock,
		Lambda,
		Robot,
		Lift,
		OpenedLift
	}

	public class Map
	{
		private MapCell[,] map;

		public Map(string[] lines)
		{
			throw new NotImplementedException();
		}

		public MapCell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public Map Move(RobotMove move)
		{
			throw new NotImplementedException();
		}
	}
}