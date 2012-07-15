using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Logic;
using NUnit.Framework;

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
		public void TestBackTrackingGreedyBot()
		{
			TestBrains(() => new BackTrakingGreedyBot(), MapsDir);
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

		private void TestBrains(Func<RobotAI> botFactory, string dir)
		{
			var now = DateTime.Now;
			long sum = 0;
			var typeBot = botFactory();
			using (var writer = new StreamWriter(Path.Combine(TestsDir, typeBot.GetType().Name + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt")))
			{

				WriteLineAndShow(writer, typeBot.GetType().Name + " " + now.ToString("yyyy-MM-dd HH:mm:ss"));
				WriteLineAndShow(writer);

				WriteLineAndShow(writer, "file".PadRight(FilenamePadding) + "score".PadRight(ValuePadding) + "moves".PadRight(ValuePadding) +"state".PadRight(ValuePadding) + "ms".PadRight(ValuePadding));
				foreach (var file in Directory.GetFiles(dir, "*.map.txt"))
				{
					
					var lines = File.ReadAllLines(file);
					WriteAndShow(writer, Path.GetFileName(file).PadRight(FilenamePadding));

					var bot = botFactory();
					Map map = new Map(lines);

					var robotMove = RobotMove.Wait;
					int movesCount = 0;

					var builder = new StringBuilder();
					var timer = Stopwatch.StartNew();
					var botWrapper = new BotWithBestMomentsMemory(bot);
					while(robotMove != RobotMove.Abort && map.State == CheckResult.Nothing)
					{
						robotMove = (timer.Elapsed.TotalSeconds < 150) ? botWrapper.NextMove(map) : RobotMove.Abort;
						movesCount++;

						map = map.Move(robotMove);
						botWrapper.UpdateBestSolution(map);
					}

					builder.Append(botWrapper.GetBestMoves());
					sum += botWrapper.BestScore;
					WriteAndShow(writer, botWrapper.BestScore.ToString().PadRight(ValuePadding) + botWrapper.BestMovesSequenceLen.ToString().PadRight(ValuePadding) + botWrapper.BestMovesEndState.ToString().PadRight(ValuePadding) + timer.ElapsedMilliseconds.ToString().PadRight(ValuePadding));
					WriteLineAndShow(writer, builder.ToString());
				}
			}
			Console.WriteLine(sum);
		}

		private void WriteLineAndShow(StreamWriter writer, string text = null)
		{
			WriteAndShow(writer, text + "\r\n");
		}

		private void WriteAndShow(StreamWriter writer, string text = null)
		{
			writer.Write(text);
			Console.Write(text);
		}

		private const string TestsDir = "../../../../tests";
		private const string MapsDir = "../../../../maps/tests";
		private const string PerformanceMapsDir = "../../../../maps/tests/performance";
		private const int FilenamePadding = 35;
		private const int ValuePadding = 8;
	}
}