using System;
using System.Collections.Generic;

namespace Logic
{
	public class BackTrakingGreedyBot : RobotAI
	{
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
			var alreadyBanned = new HashSet<Vector>();
			Vector toBan = null;
			long bestScores = long.MinValue;
			while(true)
			{
				if(StopNow) return;
				var moves = GetMoves(map, toBan);
				if(moves == null) return;
				if(bestScores < moves.Item2)
				{
					bestScores = moves.Item2;
					bestMoves = moves.Item1;
				}
				toBan = GetBanned(map, alreadyBanned);
				if(toBan != null)
					alreadyBanned.Add(toBan);
				else
					return;
			}
		}

		private Vector GetBanned(Map map, HashSet<Vector> alreadyBanned)
		{
			//if(alreadyBanned.Count < map.TotalLambdaCount)
				return GetBannedLambdas(map, alreadyBanned);
			//return GetBannedTrampolines(map, alreadyBanned);
		}

		private Vector GetBannedTrampolines(Map map, HashSet<Vector> alreadyBanned)
		{
			return null;
		}

		private Vector GetBannedLambdas(Map map, HashSet<Vector> alreadyBanned)
		{
			for(int i = 0; i < map.Width; i++)
				for(int j = 0; j < map.Height; j++)
					if(map.GetCell(i, j) == MapCell.Lambda)
					{
						var current = new Vector(i, j);
						if(!alreadyBanned.Contains(current))
							return current;
					}
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