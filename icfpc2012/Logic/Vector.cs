using System;
using System.Collections.Generic;

namespace Logic
{
	public class Vector : IEquatable<Vector>
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
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Vector)) return false;
			return Equals((Vector)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Y;
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

		public bool Equals(Vector other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.X == X && other.Y == Y;
		}

		public static bool operator ==(Vector left, Vector right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Vector left, Vector right)
		{
			return !Equals(left, right);
		}
	}

	public class VectorComparer : IComparer<Vector>
	{
		public int Compare(Vector lhs, Vector rhs)
		{
			return lhs.Y > rhs.Y
				? 1
				: lhs.Y < rhs.Y
					? -1
					: lhs.X < rhs.X
						? 1
						: lhs.X > rhs.X
							? -1
							: 0;
		}
	}

	public class TupleVectorComparer : IComparer<Tuple<Vector, Vector, MapCell>>
	{
		static readonly VectorComparer comparer = new VectorComparer();

		public int Compare(Tuple<Vector, Vector, MapCell> x, Tuple<Vector, Vector, MapCell> y)
		{
			return comparer.Compare(x.Item1, y.Item1) != 0
					? comparer.Compare(x.Item1, y.Item1)
					: comparer.Compare(x.Item2, y.Item2);
		}
	}
}