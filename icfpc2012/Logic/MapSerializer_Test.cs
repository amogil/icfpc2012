using NUnit.Framework;

namespace Logic
{
	[TestFixture]
	public class MapSerializer_Test
	{
		[Test]
		public static void AllElementsTest()
		{
			var mapCells = new MapCell[5,5];
			for(int i = 0; i < mapCells.GetLength(0); i++)
			{
				mapCells[i, 0] = MapCell.Wall;
				mapCells[i, mapCells.GetLength(1) - 1] = MapCell.Wall;
				mapCells[0, i] = MapCell.Wall;
				mapCells[mapCells.GetLength(0) - 1, i] = MapCell.Wall;
				mapCells[0, 0] = MapCell.ClosedLift;
				mapCells[0, 1] = MapCell.OpenedLift;
				mapCells[1, 1] = MapCell.Empty;
				mapCells[1, 2] = MapCell.Robot;
				mapCells[1, 3] = MapCell.Earth;
				mapCells[2, 1] = MapCell.Rock;
				mapCells[2, 2] = MapCell.Lambda;
			}
			var serializer = new MapSerializer();
			Assert.AreEqual(
				@"
#####
#.  #
#R\ #
O * #
L####

Water 0
Flooding 1
Waterproof 10".Trim(),
				serializer.Serialize(mapCells, water: 0, flooding: 1, waterproof: 10));
		}

		[Test]
		public static void OnlyWallsTest()
		{
			var map = new MapCell[4,4];
			for(int i = 0; i < map.GetLength(0); i++)
			{
				map[i, 0] = MapCell.Wall;
				map[i, map.GetLength(1) - 1] = MapCell.Wall;
				map[0, i] = MapCell.Wall;
				map[map.GetLength(0) - 1, i] = MapCell.Wall;
			}
			var serializer = new MapSerializer();
			Assert.AreEqual(
				@"
####
#  #
#  #
####

Water 1
Flooding 0
Waterproof 0".Trim(),
				serializer.Serialize(map, water: 1, flooding: 0, waterproof: 0));
		}

		[Test]
		public static void OrientationTest()
		{
			var map = new MapCell[1,2];
			map[0, 0] = MapCell.Wall;
			map[0, 1] = MapCell.Lambda;
			var serializer = new MapSerializer();
			Assert.AreEqual(
				@"
\
#

Water 1
Flooding 0
Waterproof 1".Trim(),
				serializer.Serialize(map, water: 1, flooding: 0, waterproof: 1));
		}
	}
}