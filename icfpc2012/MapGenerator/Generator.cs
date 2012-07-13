using Logic;

namespace MapGenerator
{
	internal class Generator
	{
		public Generator(int height, int width)
		{
			Height = height;
			Width = width;
		}

		private int Height { get; set; }
		private int Width { get; set; }

		public string Generate()
		{
			var map = new MapCell[Width,Height];
			PutWalls(map);
			var mapSerializer = new MapSerializer();
			return mapSerializer.Serialize(map);
		}

		private void PutWalls(MapCell[,] map)
		{
			for(int i = 0; i < Width; i++)
			{
				map[i, 0] = MapCell.Wall;
				map[i, Height - 1] = MapCell.Wall;
			}
			for(int j = 0; j < Height; j++)
			{
				map[0, j] = MapCell.Wall;
				map[Width - 1, j] = MapCell.Wall;
			}
		}
	}
}