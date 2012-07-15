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

		public override RobotMove NextMove(Map map)
		{
			if (currentTarget == null)
			{
				Tuple<Vector, Stack<RobotMove>> target = FindBestTarget(map);
				if (target == null) 
				{
					if (map.TotalLambdaCount > map.LambdasGathered && map.HasActiveRocks) 
						return FindSafePlace(map);
					else 
						return RobotMove.Abort; // TODO move rocks
				}
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

		private RobotMove FindSafePlace(Map map)
		{
			if (map.IsSafeMove(map.Robot, map.Robot.Add(new Vector(0, 1)), 1)) return RobotMove.Up;
			if (map.IsSafeMove(map.Robot, map.Robot, 1)) return RobotMove.Wait;
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> target = waveRun.EnumerateTargets(targetIsAnyCellNotOnlyLambda: true).FirstOrDefault();
			if (target == null) 
				return RobotMove.Abort;
			return target.Item2.Any() ? target.Item2.Peek() : RobotMove.Wait;
		}

		private bool MoveChangeMapSignificantly(RobotMove move)
		{
			return true;
		}

		private static Tuple<Vector, Stack<RobotMove>> FindBestTarget(Map map, bool checkBestIsNotBad = true)
		{
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> result = null;

			if (checkBestIsNotBad)
			{
				var orderedMoves = waveRun.EnumerateTargets().Take(9)
					.OrderBy(t => CalculateTargetBadness(t, map)).ToArray();
				result = orderedMoves.FirstOrDefault();
			}
			else result = waveRun.EnumerateTargets().FirstOrDefault();
			if (result != null) return result;
			if (waveRun.Lift != null && map[waveRun.Lift.Item1] == MapCell.OpenedLift)
				return waveRun.Lift;
			return null;
		}
		
		private static double CalculateTargetBadness(Tuple<Vector, Stack<RobotMove>> target, Map map)
		{
			double badness = 0.0;
			bool deadend = false;
			bool moved = CanMoveToTargetExactlyByPath(target.Item2, map,
				m =>
				{
					deadend = FindBestTarget(m, false) == null && m[m.Robot] != MapCell.OpenedLift;
				});
			if (moved && deadend) badness += 100;
			if (!CanMoveToTargetExactlyByPathWithNoRocksMoved(target.Item2, map))
				badness += 500;
			badness += target.Item2.Count;
			return badness;
		}

		private static bool CanMoveToTargetExactlyByPathWithNoRocksMoved(Stack<RobotMove> robotMoves, Map map)
		{
			int moved = 0;
			try
			{
				foreach (var move in robotMoves)
				{
					try
					{
						moved++;
						map = map.Move(move);
					}
					catch (GameFinishedException)
					{
						return false;
					}
					if (map.RocksFallAfterMoveTo(map.Robot))
					{
						return false;
					}
				}
				return true;
			}
			finally
			{
				for (int i = 0; i < moved; i++)
					map.Rollback();
			}
		}
		
		private static bool CanMoveToTargetExactlyByPath(Stack<RobotMove> robotMoves, Map map, Action<Map> analyseMap)
		{
			int moved = 0;
			try
			{
				foreach (var move in robotMoves)
				{
					try
					{
						moved++;
						map = map.Move(move);
					}
					catch (GameFinishedException)
					{
						return false;
					}
				}
				analyseMap(map);
				return true;
			}
			finally
			{
				for (int i = 0; i < moved; i++)
					map.Rollback();
			}
		}
	}
}