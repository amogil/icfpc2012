using System;
using System.Collections.Generic;
using System.Linq;
using Logic;

namespace Lighting
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string[] lines = Console.In.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			var map = new Map(lines);
			var bot = new TimeAwaredBackTrackingGreedyBot(140);
			var robotMoves = new List<RobotMove>();
			RobotMove robotMove;
			do
			{
				robotMove = bot.NextMove(map);
				robotMoves.Add(robotMove);
				map = map.Move(robotMove);
			} while(robotMove != RobotMove.Abort && map.State == CheckResult.Nothing);
			string result = robotMoves.Count > 0 ? new string(robotMoves.Select(move => move.ToChar()).ToArray()) : "A";
			Console.Write(result);
		}
	}
}