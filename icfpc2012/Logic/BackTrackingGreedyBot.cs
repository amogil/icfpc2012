using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class BackTrackingGreedyBot : RobotAI
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

		public override RobotMove NextMove(Map map, Vector target, SpecialTargetType type)
		{
			return NextMove(map);
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
			// Favorite lambdas
			var lambdas = new List<Vector>(map.TotalLambdaCount);
			foreach(var vector in GetBannedElement(map, cell => cell == MapCell.Lambda))
			{
				lambdas.Add(vector);
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Favorite);
			}
			//  Kamikadze way
			yield return new Tuple<Vector, SpecialTargetType>(new Vector(1, 1), SpecialTargetType.Kamikadze);
			// Favorite trampolines
			var trampolines = new List<Vector>();
			foreach(var vector in GetBannedElement(map, cell => cell.ToString().StartsWith("Trampoline")))
			{
				trampolines.Add(vector);
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Favorite);
			}
			// Banned lambdas
			foreach(var vector in lambdas)
				yield return new Tuple<Vector, SpecialTargetType>(vector, SpecialTargetType.Banned);
			// Banned trampoline
			foreach(var vector in trampolines)
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
			var bot = new BotWithBestMomentsMemory(new GreedyBot());
			RobotMove robotMove;
			Map localMap = map;
			do
			{
				if(StopNow) return null;
				robotMove = special != null ? bot.NextMove(localMap, special.Item1, special.Item2) : bot.NextMove(localMap);
				localMap = localMap.Move(robotMove);
				bot.UpdateBestSolution(localMap);
				moves.Add(robotMove);
			} while(robotMove != RobotMove.Abort && localMap.State == CheckResult.Nothing);
			return Tuple.Create(bot.GetBestMovesAsArray(), localMap.State != CheckResult.Fail ? localMap.GetScore() : long.MinValue);
		}
	}
}