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
	}
}