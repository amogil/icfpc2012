using System;
using System.Collections.Generic;
using Visualizer;

namespace Logic
{
	public class BackTrakingGreedyBot : RobotAI, IOverlay
	{
		private readonly Random random = new Random();
		private RobotMove[] bestMoves;
		private int currentMove;
		private GreedyBot greedyBot;

		public void Draw(Map map, IDrawer drawer)
		{
			greedyBot.Draw(map, drawer);
		}

		public override RobotMove NextMove(Map map)
		{
			if(bestMoves == null)
				CalcSolution(map);

			var robotMove = bestMoves[currentMove];
			currentMove += 1;
			return robotMove;
		}

		private void CalcSolution(Map map)
		{
			var count = 0;
			Vector banned = null;
			long bestScores = long.MinValue;
			do
			{
				var moves = GetMoves(map, banned);
				if(bestScores < moves.Item3)
				{
					bestScores = moves.Item3;
					bestMoves = moves.Item1;
					greedyBot = moves.Item2;
				}
				banned = GetBanned(map);
				count += 1;
			} while(count <= 5);
		}

		private Vector GetBanned(Map map)
		{
			var toBanIndex = random.Next(1, map.TotalLambdaCount);
			var current = 0;
			for(int i = 0; i < map.Width; i++)
				for(int j = 0; j < map.Height; j++)
					if(map.GetCell(i, j) == MapCell.Lambda)
						if(current == toBanIndex)
							return new Vector(i, j);
						else
							current += 1;
			return null;
		}

		private Tuple<RobotMove[], GreedyBot, long> GetMoves(Map map, Vector banned)
		{
			var moves = new List<RobotMove>();
			var bot = new GreedyBot();
			RobotMove robotMove;
			do
			{
				robotMove = banned != null ? bot.NextMove(map, banned) : bot.NextMove(map);
				map = map.Move(robotMove);
				if (map.State != CheckResult.Nothing)
					return Tuple.Create(moves.ToArray(), bot, -1L);
				moves.Add(robotMove);
			} while (robotMove != RobotMove.Abort && map.State == CheckResult.Nothing);
			var score = map.GetScore();
			return Tuple.Create(moves.ToArray(), bot, score);
		}
	}
}