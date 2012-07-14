﻿using System;
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
			TestBrains(new GreedyBot());
		}

		private void TestBrains(RobotAI bot)
		{
			var now = DateTime.Now;
			using (var writer = new StreamWriter(Path.Combine(TestsDir, bot.GetType().Name + "_" + now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt")))
			{

				WriteLineAndShow(writer, bot.GetType().Name + " " + now.ToString("yyyy-MM-dd HH:mm:ss"));
				WriteLineAndShow(writer);

				WriteLineAndShow(writer, "file".PadRight(FilenamePadding) + "score".PadRight(ValuePadding) + "moves".PadRight(ValuePadding) +"state".PadRight(ValuePadding) + "ms".PadRight(ValuePadding));
				foreach (var file in Directory.GetFiles(MapsDir, "*.map.txt"))
				{
					var lines = File.ReadAllLines(file);
					WriteAndShow(writer, Path.GetFileName(file).PadRight(FilenamePadding));

					IMap map = new Map(lines);

					var robotMove = RobotMove.Wait;
					int movesCount = 0;

					var builder = new StringBuilder();
					var timer = Stopwatch.StartNew();
					while (robotMove != RobotMove.Abort)
					{
						robotMove = (timer.Elapsed.TotalSeconds < 150) ? bot.NextMove(map) : RobotMove.Abort;
						movesCount++;

						builder.Append(robotMove.ToChar());
						try
						{
							map = map.Move(robotMove);
						}
						catch (GameFinishedException)
						{
							break;
						}
					}

					WriteAndShow(writer, map.GetScore().ToString().PadRight(ValuePadding) + map.MovesCount.ToString().PadRight(ValuePadding) + map.State.ToString().PadRight(ValuePadding) + timer.ElapsedMilliseconds.ToString().PadRight(ValuePadding));
					WriteLineAndShow(writer, builder.ToString());
				}
			}
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
		private const string MapsDir = "../../../../maps";
		private const int FilenamePadding = 24;
		private const int ValuePadding = 8;
	}
}