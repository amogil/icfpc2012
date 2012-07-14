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
			Assert.IsTrue(contest1.IsSafeMove(new Vector(5, 5), new Vector(5, 4), 1));
			Assert.IsFalse(contest1.IsSafeMove(new Vector(5, 4), new Vector(5, 3), 1));
			Assert.IsFalse(contest4.IsSafeMove(new Vector(3, 7), new Vector(3, 6), 1));
			Assert.IsTrue(contest4.IsSafeMove(new Vector(3, 6), new Vector(3, 5), 1));
		}
	}
}