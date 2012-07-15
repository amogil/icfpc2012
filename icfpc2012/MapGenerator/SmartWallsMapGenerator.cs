using System;
using System.Collections.Generic;
using System.Linq;
using Logic;

namespace MapGenerator
{
	public class SmartWallsMapGenerator : RandomMapGenerator
	{
		private readonly int wallSegmentLengthMax;
		private readonly int wallSegmentsCount;

		public SmartWallsMapGenerator(MapGeneratorOptions mapGeneratorOptions)
			: base(mapGeneratorOptions)
		{
			wallSegmentsCount = Convert.ToInt32(mapGeneratorOptions.WallCount * Math.Sqrt(mapGeneratorOptions.Width * mapGeneratorOptions.Height) / 3);
			wallSegmentLengthMax = Math.Min(mapGeneratorOptions.Width, mapGeneratorOptions.Height) / 10;
		}

		protected override Tuple<MapCell[,], Dictionary<MapCell, MapCell>> GenerateMap(MapCell[,] map)
		{
			PutLift(map);
			PutWalls(map);
			PutElements(map, Enumerable.Repeat(MapCell.Rock, options.RockCount));
			PutElements(map, Enumerable.Repeat(MapCell.Earth, options.EarthCount));
			PutElements(map, Enumerable.Repeat(MapCell.Lambda, options.LambdaCount));
			PutElements(map, Enumerable.Repeat(MapCell.Beard, options.BeardCount));
			PutElements(map, Enumerable.Repeat(MapCell.Razor, options.MapRazorCount));
			PutElements(map, Enumerable.Repeat(MapCell.LambdaRock, options.HighRockCount));
			var trampToTarget = PutTrampolines(map);
			PutElements(map, new[] {MapCell.Robot});
			return Tuple.Create(map, trampToTarget);
		}

		private void PutWalls(MapCell[,] mapCells)
		{
			for(int k = 0; k < wallSegmentsCount; k++)
			{
				var startPointX = random.Next(1, options.Width - 1);
				var startPointY = random.Next(1, options.Height - 1);

				var wallSegmentLength = random.Next(1, wallSegmentLengthMax + 1);
				var wallSegmentDirection = (WallDirection) random.Next(0, 4);

				int endPointX;
				int endPointY;
				switch(wallSegmentDirection)
				{
					case WallDirection.Top:
						endPointX = startPointX;
						endPointY = startPointY - wallSegmentLength;
						break;
					case WallDirection.Bottom:
						endPointX = startPointX;
						endPointY = startPointY + wallSegmentLength;
						break;
					case WallDirection.Left:
						endPointX = startPointX - wallSegmentLength;
						endPointY = startPointY;
						break;
					case WallDirection.Right:
						endPointX = startPointX + wallSegmentLength;
						endPointY = startPointY;
						break;
					default:
						throw new ArgumentException("");
				}

				for(int i = startPointX; i <= endPointX; i++)
					for(int j = startPointY; j <= endPointY; j++)
						if(i > 0 && j > 0 && i < options.Width && j < options.Height)
							mapCells[i, j] = MapCell.Wall;
			}
		}

		#region Nested type: WallDirection

		private enum WallDirection
		{
			Top = 0,
			Right = 1,
			Bottom = 2,
			Left = 3
		}

		#endregion
	}
}