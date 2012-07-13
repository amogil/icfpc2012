using System;
using System.Text;

namespace Logic
{
	public class MapSerializer
	{
		public string Serialize(MapCell[,] map)
		{
			var builder = new StringBuilder();
			for(int i = 0; i < map.GetLength(0); i++)
			{
				for(int j = 0; j < map.GetLength(1); j++)
					builder.Append(GetCellChar(map[i, j]));
				if(i < map.GetLength(0) - 1)
					builder.Append("\n");
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