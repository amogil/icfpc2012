using System;
using System.Diagnostics;
using System.IO;
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
			TestBrains(new GreedyBot());
		}

		private void TestBrains(RobotAI bot)
		{
			Console.WriteLine(bot.GetType().Name + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			Console.WriteLine();

			Console.WriteLine("file".PadRight(FilenamePadding) + "score".PadRight(ValuePadding) + "moves".PadRight(ValuePadding) + "state".PadRight(ValuePadding) + "ms");
			foreach(var file in Directory.GetFiles(MapsDir, "*.map.txt"))
			{
				var lines = File.ReadAllLines(file);
				Console.Write(Path.GetFileName(file).PadRight(FilenamePadding));

				IMap map = new Map(lines);

				var robotMove = RobotMove.Wait;
				int movesCount = 0;

				var timer = Stopwatch.StartNew();
				while(robotMove != RobotMove.Abort)
				{
					robotMove = (timer.Elapsed.TotalSeconds < 150) ? bot.NextMove(map) : RobotMove.Abort;
					movesCount++;
					
					//Console.Write(robotMove.ToChar());
					try
					{
						map = map.Move(robotMove);
					}
					catch(GameFinishedException)
					{
						break;
					}
				}

				Console.WriteLine(map.GetScore().ToString().PadRight(ValuePadding) + map.MovesCount.ToString().PadRight(ValuePadding) + map.State.ToString().PadRight(ValuePadding) + timer.ElapsedMilliseconds);
			}
		}

		private const string MapsDir = "../../../../maps";
		private const int FilenamePadding = 24;
		private const int ValuePadding = 8;
	}
}