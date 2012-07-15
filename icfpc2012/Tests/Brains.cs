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
			TestBrains(new GreedyBot(), MapsDir);
		}

		[Test, Explicit]
		public void TestPerformanceGreedyBot()
		{
			TestBrains(new GreedyBot(), PerformanceMapsDir);
		}

		private void TestBrains(RobotAI bot, string dir)
		{
			var now = DateTime.Now;
			long sum = 0;
			using (var writer = new StreamWriter(Path.Combine(TestsDir, bot.GetType().Name + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt")))
			{

				WriteLineAndShow(writer, bot.GetType().Name + " " + now.ToString("yyyy-MM-dd HH:mm:ss"));
				WriteLineAndShow(writer);

				WriteLineAndShow(writer, "file".PadRight(FilenamePadding) + "score".PadRight(ValuePadding) + "moves".PadRight(ValuePadding) +"state".PadRight(ValuePadding) + "ms".PadRight(ValuePadding));
				foreach (var file in Directory.GetFiles(dir, "*.map.txt"))
				{
					var lines = File.ReadAllLines(file);
					WriteAndShow(writer, Path.GetFileName(file).PadRight(FilenamePadding));

					Map map = new Map(lines);

					var robotMove = RobotMove.Wait;
					int movesCount = 0;

					var builder = new StringBuilder();
					var timer = Stopwatch.StartNew();
					while(robotMove != RobotMove.Abort && map.State == CheckResult.Nothing)
					{
						robotMove = (timer.Elapsed.TotalSeconds < 150) ? bot.NextMove(map) : RobotMove.Abort;
						movesCount++;

						builder.Append(robotMove.ToChar());
						map = map.Move(robotMove);
					}

					sum += map.GetScore();
					WriteAndShow(writer, map.GetScore().ToString().PadRight(ValuePadding) + map.MovesCount.ToString().PadRight(ValuePadding) + map.State.ToString().PadRight(ValuePadding) + timer.ElapsedMilliseconds.ToString().PadRight(ValuePadding));
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
		private const int FilenamePadding = 24;
		private const int ValuePadding = 8;
	}
}