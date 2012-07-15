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

		public IEnumerable<Tuple<Vector, Stack<RobotMove>>> EnumerateTargets()
		{
			var q = new Queue<WaveCell>();
			q.Enqueue(new WaveCell(startPosition, 0, null, RobotMove.Wait));
			var used = new HashSet<Vector>();
			used.Add(startPosition);

			while (q.Any())
			{
				var cell = q.Dequeue();
				MapCell toCell = map[cell.Pos];
				if (toCell == MapCell.Lambda) yield return CreateTarget(cell);
				if (toCell == MapCell.OpenedLift || toCell == MapCell.ClosedLift) Lift = CreateTarget(cell);
				foreach (var move in new[]{RobotMove.Down, RobotMove.Left, RobotMove.Right, RobotMove.Up, })
				{
					Vector newPos = cell.Pos.Add(move.ToVector());
					if (!map.IsValidMoveWithoutMovingRocks(cell.Pos, newPos)) continue;
					newPos = map.GetTrampolineTarget(newPos);
					if (!used.Contains(newPos) && map.IsSafeMove(cell.Pos, newPos, 1 + cell.StepNumber))
					{
						q.Enqueue(new WaveCell(newPos, cell.StepNumber + 1, cell, move));
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
				cell = cell.PrevCell;
			}
			return Tuple.Create(targetCell.Pos, moves);
		}

		private class WaveCell
		{
			public WaveCell(Vector pos, int stepNumber, WaveCell prevCell, RobotMove move)
			{
				Pos = pos;
				StepNumber = stepNumber;
				PrevCell = prevCell;
				Move = move;
			}

			public readonly Vector Pos;
			public readonly int StepNumber;
			public readonly RobotMove Move;
			public readonly WaveCell PrevCell;
		}
	}
}