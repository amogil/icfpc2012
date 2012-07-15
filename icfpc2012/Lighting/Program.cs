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
			Map map = new Map(lines);
			var bot = new GreedyBot();
			var timer = Stopwatch.StartNew();
			RobotMove robotMove = RobotMove.Wait;

			var path = "";
			var maxPath = new Tuple<long, string>(0, "A");

			while(robotMove != RobotMove.Abort)
			{
				robotMove = (timer.Elapsed.TotalSeconds < 150) ? bot.NextMove(map) : RobotMove.Abort;
				//Console.Write(robotMove.ToChar());
				path += robotMove.ToChar();
				try
				{
					map = map.Move(robotMove);
					map.Move(RobotMove.Abort);
					if(maxPath.Item1 < map.GetScore())
						maxPath = new Tuple<long, string>(map.GetScore(), path + "A");
					map.Rollback();
				}
				catch(GameFinishedException)
				{
					Console.WriteLine(maxPath.Item2);
					return;
				}
			}
		}
	}
}
