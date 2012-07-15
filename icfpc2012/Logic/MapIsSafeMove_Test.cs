using NUnit.Framework;

namespace Logic
{
	[TestFixture]
	public class MapIsSafeMove_Test
	{
		[Test]
		public void Test()
		{
			Map contest1 = WellKnownMaps.Contest1();
			Map contest4 = WellKnownMaps.Contest4();
			Assert.IsTrue(contest1.IsSafeMove(new Vector(5, 5), new Vector(5, 4), 1, 10));
			Assert.IsFalse(contest1.IsSafeMove(new Vector(5, 4), new Vector(5, 3), 1, 10));
			Assert.IsFalse(contest4.IsSafeMove(new Vector(3, 7), new Vector(3, 6), 1, 10));
			Assert.IsTrue(contest4.IsSafeMove(new Vector(3, 6), new Vector(3, 5), 1, 10));
		}

		[TestCase("xflood1", "2 5", RobotMove.Down, 0, 0, Result = false)]
		[TestCase("xflood1", "2 5", RobotMove.Down, 0, 1, Result = true)]
		[TestCase("xflood1", "2 5", RobotMove.Down, 0, 2, Result = true)]
		[TestCase("xflood1", "2 4", RobotMove.Down, 0, 0, Result = false)]
		[TestCase("xflood1", "2 4", RobotMove.Down, 0, 1, Result = true)]
		[TestCase("xflood1", "2 4", RobotMove.Up, 0, 0, Result = true)]
		[TestCase("xflood1", "2 4", RobotMove.Wait, 0, 0, Result = false)]
		[TestCase("xflood1", "2 4", RobotMove.Wait, 0, 1, Result = true)]
		[TestCase("xflood5", "2 7", RobotMove.Down, 3, 0, Result = false)]
		[TestCase("xflood5", "2 7", RobotMove.Down, 2, 1, Result = true)]
		[TestCase("xflood5", "1 4", RobotMove.Up, 0, 0, Result = true)]
		public bool IsSafe(string mapName, string from, RobotMove move, int movesDone, int waterproofLeft)
		{
			Map map = WellKnownMaps.LoadMap(mapName);
			return map.IsSafeMove(from, ((Vector)from).Add(move.ToVector()), movesDone, waterproofLeft);
		}

		[TestCase(0, 1, 0, Result = 0)]
		[TestCase(0, 1, 1, Result = 1)]
		[TestCase(0, 1, 2, Result = 2)]
		[TestCase(0, 2, 0, Result = 0)]
		[TestCase(0, 2, 1, Result = 0)]
		[TestCase(0, 2, 2, Result = 1)]
		[TestCase(0, 2, 3, Result = 1)]
		[TestCase(0, 2, 4, Result = 2)]
		[TestCase(0, 2, 5, Result = 2)]
		[TestCase(1, 2, 5, Result = 3)]
		[TestCase(1, 0, 0, Result = 1)]
		[TestCase(1, 0, 100500, Result = 1)]
		public int TestWaterLevel(int water, int flooding, int updateNumber)
		{
			return flooding == 0 ? water : updateNumber / flooding + water;
		}
	}
}