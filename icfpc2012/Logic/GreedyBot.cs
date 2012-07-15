using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Visualizer;

namespace Logic
{
	public class GreedyBot : RobotAI, IOverlay
	{
		private Vector currentTarget;
		private Stack<RobotMove> plan;
		private Tuple<Vector, Stack<RobotMove>> moveRockTarget;
		private Vector lastRobotPos;

		/*
		 * 
		 */

		public override RobotMove NextMove(Map map)
		{
			lastRobotPos = map.Robot;
			moveRockTarget = null;
			if (currentTarget == null)
			{
				Tuple<Vector, Stack<RobotMove>> target = FindBestTarget(map);
				if (target == null) 
				{
					if (map.TotalLambdaCount > map.LambdasGathered && map.HasActiveRocks) 
						return FindSafePlace(map);
					else
						return FindMovableRock(map); // TODO move rocks
						//return RobotMove.Abort;
				}
				currentTarget = target.Item1;
				plan = target.Item2;
			}
			RobotMove move = plan.PeekAndPop();
			if (!plan.Any() || MoveChangeMapSignificantly(map, move))
			{
				currentTarget = null;
				plan = null;
			}
			return move;
		}

		private RobotMove FindMovableRock(Map map)
		{
			//R* 

			// *R
			
			//*
			//R

			//*
			//..

			// *
			//..

			//.* 
			//

			// *.

			//.*.
			//A A

			
			var left = new Vector(-1, 0);
			var right = new Vector(1, 0);
			var up = new Vector(0, 1);

			if(map.Flooding == 0 || map.WaterproofLeft > 0)
			{
				var leftRobot = map.Robot.Add(left);
				var rightRobot = map.Robot.Add(right);
				var upRobot = map.Robot.Add(up);

				if (map[leftRobot] == MapCell.Rock && map[leftRobot.Add(left)] == MapCell.Empty)
					return RobotMove.Left;
				if (map[rightRobot] == MapCell.Rock && map[rightRobot.Add(right)] == MapCell.Empty)
					return RobotMove.Right;

				if (map[upRobot] == MapCell.Rock && map[leftRobot].IsMovable())
					return RobotMove.Left;
				if (map[upRobot] == MapCell.Rock && map[rightRobot].IsMovable())
					return RobotMove.Right;
			}

			var waveRun = new WaveRun(map, map.Robot);
			moveRockTarget = waveRun.EnumerateTargets(
				(lmap, position, used) =>
					{
						if (lmap[position.Add(up)] == MapCell.Rock && (lmap[position.Add(left)].IsMovable() || lmap[position.Add(right)].IsMovable()))
							return true;
						if (lmap[position].IsMovable() && lmap[position.Add(left)] == MapCell.Rock && lmap[position.Add(left).Add(left)].IsRockMovable())
							return true;
						if (lmap[position].IsMovable() && lmap[position.Add(right)] == MapCell.Rock && lmap[position.Add(right).Add(right)] .IsRockMovable())
							return true;
						if (lmap[position] == MapCell.Earth && lmap[position.Add(right)] == MapCell.Rock
								&& lmap[position.Add(right).Add(right)] != MapCell.Wall && lmap[position.Add(right).Add(right)] != MapCell.Rock)
							return true;
						if (lmap[position] == MapCell.Earth && lmap[position.Add(left)] == MapCell.Rock
								&& lmap[position.Add(right).Add(right)] != MapCell.Wall && lmap[position.Add(right).Add(right)] != MapCell.Rock)
							return true;
						return false;
					}).FirstOrDefault();

			if (moveRockTarget == null)
				return RobotMove.Abort;

			return moveRockTarget.Item2.Any() ? moveRockTarget.Item2.Peek() : RobotMove.Abort;
		}

		private RobotMove FindSafePlace(Map map)
		{
//			if (map.IsSafeMove(map.Robot, map.Robot.Add(new Vector(0, 1)), 1, map.WaterproofLeft)) return RobotMove.Up;
			if (map.IsSafeMove(map.Robot, map.Robot, 0, map.WaterproofLeft)) return RobotMove.Wait;
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> target = waveRun.EnumerateTargets((lmap, position) => true).FirstOrDefault();
			if (target == null) 
				return RobotMove.Abort;
			return target.Item2.Any() ? target.Item2.Peek() : RobotMove.Wait;
		}

		private bool MoveChangeMapSignificantly(Map map, RobotMove move)
		{
			return true;
//			return map.HasActiveRocks || map.RocksFallAfterMoveTo(map.Robot.Add(move.ToVector()));
		}

		private static Tuple<Vector, Stack<RobotMove>> FindBestTarget(Map map, bool checkBestIsNotBad = true)
		{
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> result = null;

			if (checkBestIsNotBad)
			{
				var orderedMoves = waveRun.EnumerateTargets((lmap, pos) => lmap[pos] == MapCell.Lambda).Take(9)
					.OrderBy(t => CalculateTargetBadness(t, map)).ToArray();
				result = orderedMoves.FirstOrDefault();
			}
			else result = waveRun.EnumerateTargets((lmap, pos) => lmap[pos] == MapCell.Lambda).FirstOrDefault();
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

		public void Draw(Map map, IDrawer drawer)
		{
			if (moveRockTarget == null) return;
			drawer.AddStyle("rock", "Brown");
			drawer.DrawTarget(map, lastRobotPos, "rock", moveRockTarget);
		}
	}
}