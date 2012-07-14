using System;
using Logic;

namespace MapGenerator
{
	internal class RandomMapGenerator
	{
		private readonly MapGeneratorOptions options;
		private readonly Random random = new Random();

		public RandomMapGenerator(MapGeneratorOptions options)
		{
			this.options = options;
		}

		public string Generate()
		{
			var mapCells = new MapCell[options.Width,options.Height];
			PutBordersWalls(mapCells);
			PutLift(mapCells);
			PutElements(mapCells, options.RockCount, MapCell.Rock);
			PutElements(mapCells, options.EarthCount, MapCell.Earth);
			PutElements(mapCells, options.WallCount, MapCell.Wall);
			PutElements(mapCells, options.LambdaCount, MapCell.Lambda);
			PutElements(mapCells, 1, MapCell.Robot);
			return new MapSerializer().Serialize(mapCells, options.WaterLevel, options.Flooding, options.Waterproof);
		}

		private void PutElements(MapCell[,] map, int count, MapCell mapCell)
		{
			for(int i = 0; i < count; i++)
				PutElement(map, mapCell);
		}

		private void PutElement(MapCell[,] map, MapCell mapCell)
		{
			var indexX = random.Next(1, map.GetLength(0) - 1);
			var indexY = random.Next(1, map.GetLength(1) - 1);
			if(map[indexX, indexY] == MapCell.Empty)
				map[indexX, indexY] = mapCell;
			else
				PutElement(map, mapCell);
		}

		private void PutLift(MapCell[,] map)
		{
			if(options.HasLift)
			{
				var indexX = random.Next(0, map.GetLength(0));
				var indexY = random.Next(0, map.GetLength(1));
				if(map[indexX, indexY] == MapCell.Empty
				   || (IsSurroundWall(map, indexX, indexY) && !IsCornerBlock(map, indexX, indexY)))
				{
					map[indexX, indexY] = MapCell.ClosedLift;
				}
				else
					PutLift(map);
			}
		}

		private static bool IsSurroundWall(MapCell[,] map, int indexX, int indexY)
		{
			return map[indexX, indexY] == MapCell.Wall
			       && (indexX == 0 || indexX == map.GetLength(0) - 1 || indexY == 0 || indexY == map.GetLength(1) - 1);
		}

		public static bool IsCornerBlock(MapCell[,] map, int indexX, int indexY)
		{
			return (indexX == 0 && indexY == 0)
			       || (indexX == map.GetLength(0) - 1 && indexY == 0)
			       || (indexX == 0 && indexY == map.GetLength(1) - 1)
			       || (indexX == map.GetLength(0) - 1 && indexY == map.GetLength(1) - 1);
		}

		private void PutBordersWalls(MapCell[,] map)
		{
			for(int i = 0; i < options.Width; i++)
			{
				map[i, 0] = MapCell.Wall;
				map[i, options.Height - 1] = MapCell.Wall;
			}
			for(int j = 0; j < options.Height; j++)
			{
				map[0, j] = MapCell.Wall;
				map[options.Width - 1, j] = MapCell.Wall;
			}
		}
	}
}