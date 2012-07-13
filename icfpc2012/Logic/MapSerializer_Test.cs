using NUnit.Framework;

namespace Logic
{
	[TestFixture]
	public class MapSerializer_Test
	{
		[Test]
		public static void AllElementsTest()
		{
			var map = new MapCell[5, 5];
			for (int i = 0; i < map.GetLength(0); i++)
			{
				map[i, 0] = MapCell.Wall;
				map[i, map.GetLength(1) - 1] = MapCell.Wall;
				map[0, i] = MapCell.Wall;
				map[map.GetLength(0) - 1, i] = MapCell.Wall;
				map[0, 0] = MapCell.ClosedLift;
				map[0, 1] = MapCell.OpenedLift;
				map[1, 1] = MapCell.Empty;
				map[1, 2] = MapCell.Robot;
				map[1, 3] = MapCell.Earth;
				map[2, 1] = MapCell.Rock;
				map[2, 2] = MapCell.Lambda;
			}
			var serializer = new MapSerializer();
			Assert.AreEqual(
				@"
#####
#.  #
#R\ #
O * #
L####".Trim(), serializer.Serialize(map));

		}
		[Test]
		public static void OrientationTest()
		{
			var map = new MapCell[1, 2];
			map[0, 0] = MapCell.Wall;
			map[0, 1] = MapCell.Lambda;
			var serializer = new MapSerializer();
			Assert.AreEqual(
				@"
\
#
".Trim(), serializer.Serialize(map));
		}

		[Test]
		public static void OnlyWallsTest()
		{
			var map = new MapCell[4,4];
			for (int i = 0; i < map.GetLength(0); i++)
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
####".Trim(), serializer.Serialize(map));
		}
	}
}