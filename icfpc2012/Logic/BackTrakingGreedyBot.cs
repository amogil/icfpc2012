using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class BackTrakingGreedyBot : RobotAI
	{
		private RobotMove[] bestMoves;
		private long bestScores = long.MinValue;
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
			if(GetItBaby(map, null)) return;

			GetSpecial(map).Any(special => GetItBaby(map, special));
		}

		private bool GetItBaby(Map map, Tuple<Vector, SpecialTargetType> special)
		{
			if(StopNow) return true;
			var moves = GetMoves(map, special);
			if(moves == null) return true;
			if(bestScores < moves.Item2)
			{
				bestScores = moves.Item2;
				bestMoves = moves.Item1;
			}
			return false;
		}

		private IEnumerable<Tuple<Vector, SpecialTargetType>> GetSpecial(Map map)
		{
			var lambdas = new List<Vector>(map.TotalLambdaCount);
			foreach(var vector in GetBannedElement(map, cell => cell == MapCell.Lambda))
			{
				lambdas.Add(vector);
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Favorite);
			}
			foreach(var vector in lambdas)
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Banned);
			foreach(var vector in GetBannedElement(map, cell => cell.ToString().StartsWith("Trampoline")))
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Banned);
		}

		private IEnumerable<Vector> GetBannedElement(Map map, Predicate<MapCell> predicate)
		{
			for(int i = 0; i < map.Width; i++)
				for(int j = 0; j < map.Height; j++)
					if(predicate(map.GetCell(i, j)))
						yield return new Vector(i, j);
		}

		private Tuple<RobotMove[], long> GetMoves(Map map, Tuple<Vector, SpecialTargetType> special)
		{
			var moves = new List<RobotMove>();
			var bot = new GreedyBot();
			RobotMove robotMove;
			Map localMap = map;
			do
			{
				if(StopNow) return null;
				robotMove = special != null ? bot.NextMove(localMap, special.Item1, special.Item2) : bot.NextMove(localMap);
				localMap = localMap.Move(robotMove);
				moves.Add(robotMove);
			} while(robotMove != RobotMove.Abort && localMap.State == CheckResult.Nothing);
			return Tuple.Create(moves.ToArray(), localMap.State != CheckResult.Fail ? localMap.GetScore() : long.MinValue);
		}
	}
}