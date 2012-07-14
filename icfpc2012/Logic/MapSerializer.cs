using System;
using System.Text;

namespace Logic
{
	public class MapSerializer
	{
		public StringBuilder SerializeMapOnly(MapCell[,] map)
		{
			var builder = new StringBuilder();
			int xUpperBound = map.GetLength(0);
			int yUpperBound = map.GetLength(1);
			for(int y = yUpperBound - 1; y >= 0; y--)
			{
				for(int x = 0; x < xUpperBound; x++)
					builder.Append(GetCellChar(map[x, y]));
				builder.AppendLine();
			}
			return builder;
		}

		public string Serialize(MapCell[,] map, int water, int flooding, int waterproof)
		{
			var builder = SerializeMapOnly(map);
			builder.AppendLine();
			builder.AppendFormat("Water {0}", water);
			builder.AppendLine();
			builder.AppendFormat("Flooding {0}", flooding);
			builder.AppendLine();
			builder.AppendFormat("Waterproof {0}", waterproof);
			return builder.ToString();
		}

		private static char GetCellChar(MapCell mapCell)
		{
			try
			{
				return (char) mapCell;
			}
			catch (Exception)
			{
				throw new ArgumentOutOfRangeException("mapCell");
			}
		}
	}
}