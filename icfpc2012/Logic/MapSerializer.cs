using System;
using System.Text;

namespace Logic
{
	public class MapSerializer
	{
		public string Serialize(MapCell[,] map, bool skipBorder = false)
		{
			var builder = new StringBuilder();
			int yUpperBound = map.GetLength(1);
			for (int y = 0; y < yUpperBound; y++)
			{
				int xUpperBound = map.GetLength(0);
				for (int x = 0; x < xUpperBound; x++)
					builder.Append(GetCellChar(map[x, yUpperBound-y-1]));
				if(y < map.GetLength(1) - 1)
					builder.AppendLine();
			}
			return builder.ToString();
		}

		private static char GetCellChar(MapCell mapCell)
		{
			switch(mapCell)
			{
				case MapCell.Empty:
					return ' ';
				case MapCell.Earth:
					return '.';
				case MapCell.Rock:
					return '*';
				case MapCell.Lambda:
					return '\\';
				case MapCell.Wall:
					return '#';
				case MapCell.Robot:
					return 'R';
				case MapCell.ClosedLift:
					return 'L';
				case MapCell.OpenedLift:
					return 'O';
				default:
					throw new ArgumentOutOfRangeException("mapCell");
			}
		}
	}
}