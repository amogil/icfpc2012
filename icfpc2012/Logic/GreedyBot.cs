using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class GreedyBot : RobotAI
	{
		private Vector currentTarget;
		private Stack<RobotMove> plan;

		/*
		 * 
		 */

		public override RobotMove NextMove(IMap map)
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

		private static Tuple<Vector, Stack<RobotMove>> FindBestTarget(IMap map, bool checkBestIsNotBad = true)
		{
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> someTarget = null;
			foreach (Tuple<Vector, Stack<RobotMove>> target in waveRun.EnumerateTargets())
			{
				bool isBad = checkBestIsNotBad && IsBadTarget(target, map);
				if (!isBad) return target;
				else someTarget = someTarget ?? target;

			}
			if (someTarget != null) return someTarget;
			if (waveRun.Lift != null && map[waveRun.Lift.Item1] == MapCell.OpenedLift)
				return waveRun.Lift;
			return null;
		}

		private static bool IsBadTarget(Tuple<Vector, Stack<RobotMove>> target, IMap map)
		{
			bool deadend = false;
			bool moved = TryMoveToTargetFairly(
				target.Item1, target.Item2, map, 
				m =>
					{
						deadend = FindBestTarget(m, false) == null && m[m.Robot] != MapCell.OpenedLift;
					});
			return !moved || deadend;
		}

		private static bool TryMoveToTargetFairly(Vector target, Stack<RobotMove> robotMoves, IMap initialMap,
		                                          Action<IMap> analyseMap)
		{
			if (initialMap.State != CheckResult.Nothing) return false;
			IMap mapAfterOneMove = initialMap.Move(robotMoves.First());
			try
			{
				if (mapAfterOneMove.Robot.Equals(target))
				{
					analyseMap(mapAfterOneMove);
					return true;
				}
				Tuple<Vector, Stack<RobotMove>> newBestTarget = FindBestTarget(initialMap, false);
				if (newBestTarget == null) return false;
				if (newBestTarget.Item1.Equals(target))
					return TryMoveToTargetFairly(target, newBestTarget.Item2, mapAfterOneMove, analyseMap);
				return false;
			}
			finally
			{
				initialMap.LoadPreviousState();
			}
		}
	}
}