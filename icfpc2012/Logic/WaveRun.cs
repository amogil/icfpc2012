using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
	public class WaveRun
	{
		private readonly Map map;
		private readonly Vector startPosition;

		public WaveRun(Map map, Vector startPosition)
		{
			this.map = map;
			this.startPosition = startPosition;
		}

		public Tuple<Vector, Stack<RobotMove>> Lift { get; private set; }


		public IEnumerable<Tuple<Vector, Stack<RobotMove>>> EnumerateTargets(Func<Map, Vector, int, bool> isTarget)
		{
			return EnumerateTargets((lmap, pos, used, stepNumber) => isTarget(lmap, pos, stepNumber));
		}

		public IEnumerable<Tuple<Vector, Stack<RobotMove>>> EnumerateTargets(Func<Map, Vector, HashSet<Vector>, int, bool> isTarget)
		{
			var q = new Queue<WaveCell>();
			q.Enqueue(new WaveCell(startPosition, 0, null, RobotMove.Wait, map.WaterproofLeft, map.Razors));
			var used = new HashSet<Vector>();
			used.Add(startPosition);

			while (q.Any())
			{
				var cell = q.Dequeue();
				MapCell toCell = map.GetCell(cell.Pos);
				if (!cell.Pos.Equals(startPosition)
					&& (isTarget(map, cell.Pos, used, cell.StepNumber)))
					yield return CreateTarget(cell);
				if (toCell == MapCell.OpenedLift || toCell == MapCell.ClosedLift) 
					Lift = CreateTarget(cell);
				foreach (var move in new[]{RobotMove.Down, RobotMove.Left, RobotMove.Right, RobotMove.Up, })
				{
					Vector newPos = cell.Pos.Add(move.ToVector());
					if (!map.IsValidMoveWithoutMovingRocks(cell.Pos, newPos)) continue;

					newPos = map.GetTrampolineTarget(newPos);
					var mapcell = map.GetCell(newPos);

					if ((!used.Contains(newPos) && (map.GetCell(newPos) == MapCell.OpenedLift || map.IsSafeMove(cell.Pos, newPos, cell.StepNumber + 1, cell.WaterproofLeft)))
						&& (mapcell != MapCell.Beard || cell.RazorsLeft > 0))
					{
						var wp = map.IsInWater(cell.StepNumber, newPos.Y) ? cell.WaterproofLeft - 1 : map.Waterproof;
						q.Enqueue(new WaveCell(newPos, cell.StepNumber + 1, cell, move, wp, mapcell == MapCell.Beard ? map.Razors - 1: map.Razors));
						used.Add(newPos);
					}
				}
			}
		}

		private Tuple<Vector, Stack<RobotMove>> CreateTarget(WaveCell targetCell)
		{
			var moves = new Stack<RobotMove>(targetCell.StepNumber+1);
			var cell = targetCell;
			while (cell.PrevCell != null)
			{
				moves.Push(cell.Move);
				if(cell.PrevCell.RazorsLeft != cell.RazorsLeft)
					moves.Push(RobotMove.CutBeard);
				cell = cell.PrevCell;
			}
			return Tuple.Create(targetCell.Pos, moves);
		}

		private class WaveCell
		{
			public WaveCell(Vector pos, int stepNumber, WaveCell prevCell, RobotMove move, int waterproofLeft, int razorsLeft)
			{
				Pos = pos;
				StepNumber = stepNumber;
				PrevCell = prevCell;
				Move = move;
				WaterproofLeft = waterproofLeft;
				RazorsLeft = razorsLeft;
			}

			public readonly int RazorsLeft;
			public readonly Vector Pos;
			public readonly int StepNumber;
			public readonly RobotMove Move;
			public readonly WaveCell PrevCell;
			public readonly int WaterproofLeft;
		}
	}
}