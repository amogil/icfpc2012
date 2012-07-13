﻿using System;
using Logic;

namespace MapGenerator
{
	internal class Generator
	{
		private readonly Random random = new Random();

		public Generator(int height, int width, bool hasLift = true, int rocksCount = 0, int earthCount = 0, int wallCount = 0,
		                 int lambdaCount = 1)
		{
			Height = height;
			Width = width;
			HasLift = hasLift;
			RocksCount = rocksCount;
			EarthCount = earthCount;
			WallCount = wallCount;
			LambdaCount = lambdaCount;
		}

		private int Height { get; set; }
		private int Width { get; set; }
		private bool HasLift { get; set; }
		private int RocksCount { get; set; }
		private int EarthCount { get; set; }
		private int WallCount { get; set; }
		private int LambdaCount { get; set; }

		public string Generate()
		{
			var map = new MapCell[Width,Height];
			PutBordersWalls(map);
			PutLift(map);
			PutElements(map, RocksCount, MapCell.Rock);
			PutElements(map, EarthCount, MapCell.Earth);
			PutElements(map, WallCount, MapCell.Wall);
			PutElements(map, LambdaCount, MapCell.Lambda);
			PutElements(map, 1, MapCell.Robot);
			var mapSerializer = new MapSerializer();
			return mapSerializer.Serialize(map);
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
				PutElement(map, MapCell.Rock);
		}

		private void PutLift(MapCell[,] map)
		{
			if(HasLift)
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