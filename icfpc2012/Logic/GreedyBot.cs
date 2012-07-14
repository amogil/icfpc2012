using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
	public class GreedyBot : RobotAI
	{
		public GreedyBot(Map map) : base(map)
		{
		}

		public override IEnumerable<RobotMove> GetMoves()
		{
			while(true)
			{
				Tuple<Vector, RobotMove[]> target = FindClosestLambda();
				if (target == null) yield return RobotMove.Abort;
				else
				{
					foreach (var move in target.Item2)
						yield return move;
				}
			}
		}

		private Tuple<Vector, RobotMove[]> FindClosestLambda()
		{
			var waveRun = new WaveRun(Map, new Vector(Map.RobotX, Map.RobotY));
			foreach (var target in waveRun.EnumerateTargets())
			{
				return target;
			}
			if (waveRun.Lift != null) return waveRun.Lift;
			return null;
		}
	}

}