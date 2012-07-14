using System;
using Logic;

namespace MapGenerator
{
	public class PatternMapGenerator : RandomMapGenerator
	{
		private readonly MapGeneratorOptions options;
		private readonly int wallSegmentLengthMax;
		private readonly int wallSegmentsCount;

		public PatternMapGenerator(MapGeneratorOptions mapGeneratorOptions)
			: base(mapGeneratorOptions)
		{
			options = mapGeneratorOptions;
			wallSegmentsCount = mapGeneratorOptions.Width * mapGeneratorOptions.Height / 7;
			wallSegmentLengthMax = Math.Min(mapGeneratorOptions.Width, mapGeneratorOptions.Height) / 10;
		}

		public override string Generate()
		{
			var mapCells = new MapCell[options.Width,options.Height];
			PutBordersWalls(mapCells);
			PutLift(mapCells);
			PutWalls(mapCells);
			PutElements(mapCells, options.RockCount, MapCell.Rock);
			PutElements(mapCells, options.EarthCount, MapCell.Earth);
			PutElements(mapCells, options.WallCount, MapCell.Wall);
			PutElements(mapCells, options.LambdaCount, MapCell.Lambda);
			PutElements(mapCells, 1, MapCell.Robot);
			return new MapSerializer().Serialize(mapCells, options.WaterLevel, options.Flooding, options.Waterproof);
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