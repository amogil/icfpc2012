using System;
using System.Collections.Generic;
using System.Linq;
using Logic;

namespace MapGenerator
{
	public class RandomMapGenerator : IMapGenerator
	{
		protected readonly MapGeneratorOptions options;
		protected readonly Random random = new Random();

		public RandomMapGenerator(MapGeneratorOptions options)
		{
			this.options = options;
		}

		public string Generate()
		{
			var initMap = CreateEmptyMap();
			PutBordersWalls(initMap);
			var mapInfo = GenerateMap(initMap);
			return new MapSerializer().Serialize(mapInfo.Item1, options.WaterLevel, options.Flooding,
			                                     options.Waterproof, mapInfo.Item2, options.BeardGrowth,
			                                     options.PocketRazorCount);
		}

		protected virtual Tuple<MapCell[,], Dictionary<MapCell, MapCell>> GenerateMap(MapCell[,] map)
		{
			PutLift(map);
			PutElements(map, Enumerable.Repeat(MapCell.Rock, options.RockCount));
			PutElements(map, Enumerable.Repeat(MapCell.Earth, options.EarthCount));
			PutElements(map, Enumerable.Repeat(MapCell.Wall, options.WallCount));
			PutElements(map, Enumerable.Repeat(MapCell.Lambda, options.LambdaCount));
			PutElements(map, Enumerable.Repeat(MapCell.Beard, options.BeardCount));
			PutElements(map, Enumerable.Repeat(MapCell.Razor, options.MapRazorCount));
			PutElements(map, Enumerable.Repeat(MapCell.LambdaRock, options.HighRockCount));
			var trampToTarget = PutTrampolines(map);
			PutElements(map, new[] {MapCell.Robot});
			return Tuple.Create(map, trampToTarget);
		}

		protected static IEnumerable<MapCell> GetEnumValuesByNamePrefixSorted(string prefix, int count)
		{
			var names = Enum.GetNames(typeof(MapCell));
			var trampolinesNames = names
				.Where(name => name.StartsWith(prefix))
				.ToArray();
			Array.Sort(trampolinesNames);
			return trampolinesNames.Select(name => (MapCell) Enum.Parse(typeof(MapCell), name)).Take(count);
		}

		private MapCell[,] CreateEmptyMap()
		{
			var mapCells = new MapCell[options.Width,options.Height];
			for(int i = 0; i < options.Width; i++)
				for(int j = 0; j < options.Height; j++)
					mapCells[i, j] = MapCell.Empty;
			return mapCells;
		}

		protected Vector[] PutElements(MapCell[,] map, IEnumerable<MapCell> mapCells)
		{
			return mapCells.Select(mapCell => PutElement(map, mapCell)).ToArray();
		}

		protected Vector PutElement(MapCell[,] map, MapCell mapCell)
		{
			var indexX = random.Next(1, map.GetLength(0) - 1);
			var indexY = random.Next(1, map.GetLength(1) - 1);
			if(map[indexX, indexY] == MapCell.Empty)
			{
				map[indexX, indexY] = mapCell;
				return new Vector(indexX, indexY);
			}
			return PutElement(map, mapCell);
		}

		protected void PutLift(MapCell[,] map)
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

		protected static bool IsSurroundWall(MapCell[,] map, int indexX, int indexY)
		{
			return map[indexX, indexY] == MapCell.Wall
			       && (indexX == 0 || indexX == map.GetLength(0) - 1 || indexY == 0 || indexY == map.GetLength(1) - 1);
		}

		protected static bool IsCornerBlock(MapCell[,] map, int indexX, int indexY)
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

		protected Dictionary<MapCell, MapCell> PutTrampolines(MapCell[,] map)
		{
			if(options.TrampolineCount == 0) return new Dictionary<MapCell, MapCell>();
			var trampolines = PutElements(map, GetEnumValuesByNamePrefixSorted("Trampoline", options.TrampolineCount));
			var trampolinesHasOneTarget = random.Next(0, 2);
			var targetsCount = trampolinesHasOneTarget == 0 ? options.TrampolineCount : options.TrampolineCount / 2 + 1;
			var targets = PutElements(map, GetEnumValuesByNamePrefixSorted("Target", targetsCount));
			var trampToTarget = new Dictionary<MapCell, MapCell>();
			for(int i = 0; i < targets.Length; i++)
			{
				trampToTarget[map[trampolines[i].X, trampolines[i].Y]] = map[targets[i].X, targets[i].Y];
			}
			for(int i = targets.Length; i < trampolines.Length; i++)
			{
				var targetIndex = random.Next(0, targets.Length);
				trampToTarget[map[trampolines[i].X, trampolines[i].Y]] = map[targets[targetIndex].X, targets[targetIndex].Y];
			}
			return trampToTarget;
		}
	}
}