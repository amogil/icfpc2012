using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Logic;

namespace Lighting
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] lines = Console.In.ReadToEnd().Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
			var map = new Map(lines);
			var bot = new GreedyBot();
			var timer = Stopwatch.StartNew();
			RobotMove robotMove = RobotMove.Wait;
			while(robotMove != RobotMove.Abort)
			{
				robotMove = (timer.Elapsed.TotalSeconds < 150) ? bot.NextMove(map) : RobotMove.Abort;
				Console.Write(robotMove.ToChar());
				try
				{
					map = map.Move(robotMove);
				}
				catch(GameFinishedException)
				{
					return;
				}
			}
		}
	}
}
