using System;
using System.Collections.Generic;
using Logic;

namespace MapGenerator
{
	public class IsolatedMapGenerator : SmartWallsMapGenerator
	{
		public IsolatedMapGenerator(MapGeneratorOptions mapGeneratorOptions) : base(mapGeneratorOptions)
		{
		}

		protected override Tuple<MapCell[,], Dictionary<MapCell, MapCell>> GenerateMap(MapCell[,] map)
		{
			var indexX = random.Next(2, map.GetLength(0) - 3);
			var indexY = random.Next(2, map.GetLength(1) - 3);
			for(int i = 0; i < map.GetLength(0); i++)
				map[i, indexY] = MapCell.Wall;
			for(int j = 0; j < map.GetLength(1); j++)
				map[indexX, j] = MapCell.Wall;
			return base.GenerateMap(map);
		}
	}
}