using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer;

namespace Logic
{
	public class GreedyBot : RobotAI, IOverlay
	{
		private Vector bannedTarget;
		private Vector currentTarget;
		private Vector lastRobotPos;
		private Tuple<Vector, Stack<RobotMove>> moveRockTarget;
		private Stack<RobotMove> plan;

		public void Draw(Map map, IDrawer drawer)
		{
			if(moveRockTarget == null) return;
			drawer.AddStyle("rock", "Brown");
			drawer.DrawTarget(map, lastRobotPos, "rock", moveRockTarget);
		}

		public RobotMove NextMove(Map map, Vector banned)
		{
			bannedTarget = banned;
			return NextMove(map);
		}

		/*
		 * 
		 */

		public override RobotMove NextMove(Map map)
		{
			lastRobotPos = map.Robot;
			moveRockTarget = null;
			if(currentTarget == null)
			{
				Tuple<Vector, Stack<RobotMove>> target = FindBestTarget(map);
				RobotMove newmove = RobotMove.Abort;
				if(target == null && map.TotalLambdaCount > map.LambdasGathered)
					newmove = FindMovableRock(map);
				if (target == null && newmove != RobotMove.Abort)
					return newmove;
				if(target == null && newmove == RobotMove.Abort)
				{
					if(map.TotalLambdaCount > map.LambdasGathered && map.HasActiveRocks)
						return FindSafePlace(map);
					return RobotMove.Abort;
				}
				currentTarget = target.Item1;
				plan = target.Item2;
			}
			if (!plan.Any()) return RobotMove.Abort;
			RobotMove move = plan.PeekAndPop();
			if(!plan.Any() || MoveChangeMapSignificantly(map, move))
			{
				currentTarget = null;
				plan = null;
			}
			return move;
		}

		private RobotMove FindMovableRock(Map map)
		{
			var left = new Vector(-1, 0);
			var right = new Vector(1, 0);
			var up = new Vector(0, 1);

			if(map.Flooding == 0 || map.WaterproofLeft > 0)
			{
				var leftRobot = map.Robot.Add(left);
				var rightRobot = map.Robot.Add(right);
				var upRobot = map.Robot.Add(up);

				if (map.GetCell(leftRobot).IsRock() && map.GetCell(leftRobot.Add(left)) == MapCell.Empty)
					return RobotMove.Left;
				if (map.GetCell(rightRobot).IsRock() && map.GetCell(rightRobot.Add(right)) == MapCell.Empty)
					return RobotMove.Right;

				if (map.GetCell(upRobot).IsRock() && map.GetCell(leftRobot).IsMovable())
					return RobotMove.Left;
				if (map.GetCell(upRobot).IsRock() && map.GetCell(rightRobot).IsMovable())
					return RobotMove.Right;
			}

			var waveRun = new WaveRun(map, map.Robot);
			moveRockTarget = waveRun.EnumerateTargets(
				(lmap, position, used) =>
					{
						if (lmap.GetCell(position.Add(up)).IsRock() && (lmap.GetCell(position.Add(left)).IsMovable() || lmap.GetCell(position.Add(right)).IsMovable()))
							return true;
						if (lmap.GetCell(position).IsMovable() && lmap.GetCell(position.Add(left)).IsRock() && lmap.GetCell(position.Add(left).Add(left)).IsRockMovable())
							return true;
						if (lmap.GetCell(position).IsMovable() && lmap.GetCell(position.Add(right)).IsRock() && lmap.GetCell(position.Add(right).Add(right)).IsRockMovable())
							return true;
						if (lmap.GetCell(position) == MapCell.Earth && lmap.GetCell(position.Add(right)).IsRock()
								&& lmap.GetCell(position.Add(right).Add(right)) != MapCell.Wall && !lmap.GetCell(position.Add(right).Add(right)).IsRock())
							return true;
						if (lmap.GetCell(position) == MapCell.Earth && lmap.GetCell(position.Add(left)) == MapCell.Rock
								&& lmap.GetCell(position.Add(right).Add(right)) != MapCell.Wall && lmap.GetCell(position.Add(right).Add(right)) != MapCell.Rock)
							return true;
						return false;
					}).FirstOrDefault();

			if(moveRockTarget == null)
				return RobotMove.Abort;

			return moveRockTarget.Item2.Any() ? moveRockTarget.Item2.Peek() : RobotMove.Abort;
		}

		private RobotMove FindSafePlace(Map map)
		{
//			if (map.IsSafeMove(map.Robot, map.Robot.Add(new Vector(0, 1)), 1, map.WaterproofLeft)) return RobotMove.Up;
			if(map.IsSafeMove(map.Robot, map.Robot, 0, map.WaterproofLeft)) return RobotMove.Wait;
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> target = waveRun.EnumerateTargets((lmap, position, stepNumber) => true).FirstOrDefault();
			if(target == null)
				return RobotMove.Abort;
			return target.Item2.Any() ? target.Item2.Peek() : RobotMove.Wait;
		}

		private bool MoveChangeMapSignificantly(Map map, RobotMove move)
		{
			return true;
//			return map.HasActiveRocks || map.RocksFallAfterMoveTo(map.Robot.Add(move.ToVector()));
		}

		private Tuple<Vector, Stack<RobotMove>> FindBestTarget(Map map, bool checkBestIsNotBad = true)
		{
			var waveRun = new WaveRun(map, map.Robot);
			Tuple<Vector, Stack<RobotMove>> result = null;

			if(checkBestIsNotBad)
			{
				var orderedMoves = waveRun
					.EnumerateTargets((lmap, pos, stepNumber) => lmap.GetCell(pos) == MapCell.Lambda)
					.Where(tuple => bannedTarget == null || (tuple.Item1.X != bannedTarget.X && tuple.Item1.Y != bannedTarget.Y))
					.Take(9)
					.OrderBy(t => CalculateTargetBadness(t, map)).ToArray();
				result = orderedMoves.FirstOrDefault();
			}
			else result = waveRun.EnumerateTargets((lmap, pos, stepNumber) => lmap.GetCell(pos) == MapCell.Lambda).FirstOrDefault();
			if(result != null) return result;
			if(waveRun.Lift != null && map.GetCell(waveRun.Lift.Item1) == MapCell.OpenedLift)
				return waveRun.Lift;
			return null;
		}

		private double CalculateTargetBadness(Tuple<Vector, Stack<RobotMove>> target, Map map)
		{
			double badness = 0.0;
			bool deadend = false;
			bool moved = CanMoveToTargetExactlyByPath(target.Item2, map,
			                                          m =>
			                                          	{
			                                          		deadend = FindBestTarget(m, false) == null &&
			                                          		          m.GetCell(m.Robot) != MapCell.OpenedLift;
			                                          	});
			if(moved && deadend) badness += 100;
			if(!CanMoveToTargetExactlyByPathWithNoRocksMoved(target.Item2, map))
				badness += 500;
			if (map.IsInWater(target.Item2.Count, target.Item1.Y) && !CanEscapeFromUnderwater(target, map))
				badness += 500;
			badness += target.Item2.Count;
			return badness;
		}

		private static bool CanEscapeFromUnderwater(Tuple<Vector, Stack<RobotMove>> target, Map map)
		{
			foreach (var move in target.Item2)
			{
				map = map.Move(move);
				if (map.State == CheckResult.Fail)
					return false;
			}
			return 
				new WaveRun(map, map.Robot)
					.EnumerateTargets(
						(lmap, pos, moves, stepNumber) =>
							lmap.GetCell(pos) == MapCell.OpenedLift ||
							!lmap.IsInWater(stepNumber, pos.Y) && lmap.GetCell(pos).CanStepUp())
					.FirstOrDefault() != null;
		}

		private static bool CanMoveToTargetExactlyByPathWithNoRocksMoved(Stack<RobotMove> robotMoves, Map map)
		{
			foreach(var move in robotMoves)
			{
				map = map.Move(move);
				if (map.State == CheckResult.Fail)
					return false;
				if (map.RocksFallAfterMoveTo(map.Robot))
					return false;
			}
			return true;
		}

		private static bool CanMoveToTargetExactlyByPath(Stack<RobotMove> robotMoves, Map map, Action<Map> analyseMap)
		{
			foreach (var move in robotMoves)
			{
				map = map.Move(move);
				if (map.State == CheckResult.Fail)
					return false;
			}
			return true;
		}
	}
}