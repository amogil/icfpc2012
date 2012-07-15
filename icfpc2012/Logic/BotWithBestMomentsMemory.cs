using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class BotWithBestMomentsMemory // =)
	{
		private readonly RobotAI bot;
		private readonly List<RobotMove> currentMoves = new List<RobotMove>();
		private CheckResult bestMovesEndState;
		private int bestMovesSequenceLen;
		private long bestScore;

		public BotWithBestMomentsMemory(RobotAI bot)
		{
			this.bot = bot;
			bestScore = 0;
			bestMovesSequenceLen = 0; //без учета аборта
			bestMovesEndState = CheckResult.Abort;
		}

		public RobotAI Bot
		{
			get { return bot; }
		}

		public long BestScore
		{
			get { return bestScore; }
		}

		public CheckResult BestMovesEndState
		{
			get { return bestMovesEndState; }
		}

		public int BestMovesSequenceLen
		{
			get { return bestMovesSequenceLen + (EndsWithAbort(bestMovesEndState) ? 1 : 0); }
		}

		public RobotMove NextMove(Map map)
		{
			RobotMove robotMove = bot.NextMove(map);
			currentMoves.Add(robotMove);
			return robotMove;
		}

		public RobotMove NextMove(Map map, Vector target, SpecialTargetType type)
		{
			RobotMove robotMove = bot.NextMove(map, target, type);
			currentMoves.Add(robotMove);
			return robotMove;
		}

		public void UpdateBestSolution(Map map)
		{
			long score = map.GetScore();
			if(score > bestScore)
			{
				bestScore = score;
				bestMovesSequenceLen = currentMoves.Count;
				bestMovesEndState = map.State;
			}
		}

		private static bool EndsWithAbort(CheckResult checkResult)
		{
			return checkResult != CheckResult.Win && checkResult != CheckResult.Fail;
		}

		public string GetBestMoves()
		{
			var res = new string(currentMoves.Take(bestMovesSequenceLen).Select(m => m.ToChar()).ToArray());
			if(EndsWithAbort(bestMovesEndState)) res = res + "A";
			return res;
		}
	}
}