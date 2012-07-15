using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Logic;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
	[TestFixture]
	public class Brains
	{
		[Test]
		public void TestGreedyBot()
		{
			TestBrains(() => new GreedyBot(), MapsDir);
		}

		[Test]
		public void TestTimeAwareBackTrackingGreedyBot()
		{
			TestBrains(() => new TimeAwaredBackTrackingGreedyBot(140), MapsDir);
		}

		[Test, Explicit]
		public void Profiling()
		{
			var greedyBot = new GreedyBot();
			Map map = WellKnownMaps.LoadMap("tests\\performance\\random42_1000");
			while (map.State == CheckResult.Nothing)
			{
				map = map.Move(greedyBot.NextMove(map));
			}
			Console.WriteLine(map.GetScore());
		}

		[Test]
		public void Profiling2()
		{
			var greedyBot = new GreedyBot();
			Map map = WellKnownMaps.LoadMap("tests\\performance\\random19_nf_2000");
			while (map.State == CheckResult.Nothing)
			{
				map = map.Move(greedyBot.NextMove(map));
			}
			Console.WriteLine(map.GetScore());
		}

		[Test, Explicit]
		public void TestPerformanceGreedyBot()
		{
			TestBrains(() => new GreedyBot(), PerformanceMapsDir);
		}

		[Test, Explicit]
		public void TestPerformanceOnConcreteMap()
		{
			var map = new Map(Path.Combine(MapsDir, "random20_fl_50.map.txt"));
			var robotMove = RobotMove.Wait;
			var bot = new GreedyBot();
			var botWrapper = new BotWithBestMomentsMemory(bot);
			while (robotMove != RobotMove.Abort && map.State == CheckResult.Nothing)
			{
				robotMove = botWrapper.NextMove(map);
				map = map.Move(robotMove);
				botWrapper.UpdateBestSolution(map);
			}
		}

		private string[] LoadHistory(string dir, string mapName, string botName)
		{
			var filename = GetHistoryFilename(dir, mapName, botName);
			if (File.Exists(filename)) return File.ReadAllLines(filename);
			return new string[0];
		}

		private static string GetHistoryFilename(string dir, string mapName, string botName)
		{
			string filename = Path.Combine(dir, mapName + "_" + botName + ".history");
			return filename;
		}

		private void TestBrains(Func<RobotAI> botFactory, string dir)
		{
			var now = DateTime.Now;
			var typeBot = botFactory();
			string botName = typeBot.GetType().Name;
			using (var writer = new StreamWriter(Path.Combine(TestsDir, botName + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt")))
			{
				long sum = 0;
				WriteLineAndShow(writer, botName + " " + now.ToString("yyyy-MM-dd HH:mm:ss"));
				WriteLineAndShow(writer);
				WriteLineAndShow(writer, "\t  score: [W|N|A] <SCORE> (W - win, N - nothing, A - Abort)");
				WriteLineAndShow(writer);

				WriteLineAndShow(writer, 
					"map".PadRight(FilenamePadding)
					+ "ms".PadRight(ValuePadding)
					+ "ch?".PadRight(ValuePadding)
					+ "curScore".PadRight(ValuePadding) 
					+ "prevScores    ...   moves");
				foreach (var file in Directory.GetFiles(dir, "*.map.txt"))
				{
					string mapName = Path.GetFileNameWithoutExtension(file) ?? "NA";
					var lines = File.ReadAllLines(file);
					var bot = botFactory();
					var map = new Map(lines);

					var robotMove = RobotMove.Wait;

					var timer = Stopwatch.StartNew();
					var botWrapper = new BotWithBestMomentsMemory(bot);
					while(robotMove != RobotMove.Abort && map.State == CheckResult.Nothing)
					{
						robotMove = (timer.Elapsed.TotalSeconds < 150) ? botWrapper.NextMove(map) : RobotMove.Abort;

						map = map.Move(robotMove);
						botWrapper.UpdateBestSolution(map);
					}
					string[] history = LoadHistory(dir, mapName, botName);
					string result = botWrapper.BestMovesEndState.ToString()[0] + " " + botWrapper.BestScore.ToString();
					bool resultChanged = result != history.FirstOrDefault();
					WriteLineAndShow(writer, 
						mapName.PadRight(FilenamePadding)
						+ timer.ElapsedMilliseconds.ToString().PadRight(ValuePadding)
						+ (resultChanged ? "*" : "").PadRight(ValuePadding)
						+ result.PadRight(ValuePadding)
						+ String.Join(" ", history.Take(10)) + "  "
						+ botWrapper.GetBestMoves());
					if (resultChanged)
						history = new[] {result}.Concat(history).ToArray();
					File.WriteAllLines(GetHistoryFilename(dir, mapName, botName), history);
					sum += botWrapper.BestScore;
				}
				WriteLineAndShow(writer, sum.ToString());
			}
		}

		private void WriteLineAndShow(StreamWriter writer, string text = null)
		{
			writer.WriteLine(text);
			Console.WriteLine(text);
		}

		private const string TestsDir = "../../../../tests";
		private const string MapsDir = "../../../../maps/tests";
		private const string PerformanceMapsDir = "../../../../maps/tests/performance";
		private const int FilenamePadding = 30;
		private const int ValuePadding = 10;
	}
}