using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
	public class GreedyBot : RobotAI
	{
		private Vector currentTarget;
		private Stack<RobotMove> plan;
		/*
		 * 
		 */
		public override RobotMove NextMove(Map map)
		{
			if (currentTarget == null)
			{
				Tuple<Vector, Stack<RobotMove>> target = FindBestTarget(map);
				if (target == null) return RobotMove.Abort; //TODO
				currentTarget = target.Item1;
				plan = target.Item2;
			}
			RobotMove move = plan.PeekAndPop();
			if (MoveChangeMapSignificantly(move))
			{
				currentTarget = null;
				plan = null;
			}
			return move;
		}

		private bool MoveChangeMapSignificantly(RobotMove move)
		{
			return true;
		}

		private Tuple<Vector, Stack<RobotMove>> FindBestTarget(Map map)
		{
			var waveRun = new WaveRun(map, map.Robot);
			foreach (var target in waveRun.EnumerateTargets())
				return target;
			if (waveRun.Lift != null && map[waveRun.Lift.Item1] == MapCell.OpenedLift) 
				return waveRun.Lift;
			return null;
		}
	}

}