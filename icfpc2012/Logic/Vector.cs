using System;

namespace Logic
{
	public class Vector
	{
		public readonly int X, Y;

		public static implicit operator Vector(string s)
		{
			var parts = s.Split(' ');
			return new Vector(int.Parse(parts[0]), int.Parse(parts[1]));
		}

		public Vector(int x, int y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
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
				return (X*397) ^ Y;
			}
		}

		public Vector Add(Vector diff)
		{
			return new Vector(X + diff.X, Y + diff.Y);
		}

		public Vector Sub(Vector diff)
		{
			return new Vector(X - diff.X, Y - diff.Y);
		}

		public int Distance(Vector diff)
		{
			return Math.Max(Math.Abs(diff.X - X), Math.Abs(diff.Y - Y));
		}
	}
}