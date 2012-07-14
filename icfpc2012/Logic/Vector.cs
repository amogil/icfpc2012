using System.Collections.Generic;

namespace Logic
{
	public class Vector
	{
		public readonly int X, Y;

		public Vector(int x, int y)
		{
			X = x;
			Y = y;
		}

		public bool InBoard
		{
			get { return X >= 0 && X <= 7 && Y >= 0 && Y <= 7; }
		}

		public static IEnumerable<Vector> AllBoard()
		{
			for (int y = 0; y < 8; y++)
				for (int x = 0; x < 8; x++)
					yield return new Vector(x, y);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Vector;
			if (other == null) return false;
			return other.X == X && other.Y == Y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Y;
			}
		}
	}
}