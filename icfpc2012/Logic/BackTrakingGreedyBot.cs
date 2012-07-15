using System;
using System.Collections.Generic;

namespace Logic
{
	public class BackTrakingGreedyBot : RobotAI
	{
		private readonly Random random = new Random();
		private RobotMove[] bestMoves;
		private int currentMove;

		public override RobotMove NextMove(Map map)
		{
			if(bestMoves == null)
			{
				CalcSolution(map);
				if(bestMoves == null) return RobotMove.Abort;
			}

			return currentMove <= bestMoves.Length - 1 ? bestMoves[currentMove++] : RobotMove.Abort;
		}

		private void CalcSolution(Map map)
		{
			var count = 0;
			Vector banned = null;
			long bestScores = long.MinValue;
			do
			{
				if(StopNow) return;
				var moves = GetMoves(map, banned);
				if(moves == null) return;
				if(bestScores < moves.Item2)
				{
					bestScores = moves.Item2;
					bestMoves = moves.Item1;
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

		private Tuple<RobotMove[], long> GetMoves(Map map, Vector banned)
		{
			var moves = new List<RobotMove>();
			var bot = new GreedyBot();
			RobotMove robotMove;
			Map localMap = map;
			do
			{
				if(StopNow) return null;
				robotMove = banned != null ? bot.NextMove(localMap, banned) : bot.NextMove(localMap);
				localMap = localMap.Move(robotMove);
				moves.Add(robotMove);
			} while(robotMove != RobotMove.Abort && localMap.State == CheckResult.Nothing);
			return Tuple.Create(moves.ToArray(), localMap.State != CheckResult.Fail ? localMap.GetScore() : long.MinValue);
		}
	}
}