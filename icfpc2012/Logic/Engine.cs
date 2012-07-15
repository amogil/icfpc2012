using System;
using System.Collections.Generic;

namespace Logic
{
	public class Engine
	{
		public Engine(Map map)
		{
			Map = map;
		}

		public Map Map { get; private set; }

		public event Action<Map> OnMapUpdate;
		public event Action<RobotMove> OnMoveAdded;

		public virtual void RunProgram(IEnumerable<RobotMove> moves)
		{
			try
			{
				foreach (var move in moves)
					DoMove(move);
			}
			catch (GameFinishedException)
			{
			}
		}

		public void DoMove(RobotMove robotMove)
		{
			if (Map.State != CheckResult.Nothing) return;
			var newMap = Map.Move(robotMove);
			AddMove(robotMove);
			UpdateMap(newMap);
			if (newMap.State != CheckResult.Nothing)
				throw new GameFinishedException();
		}

		private void UpdateMap(Map newMap)
		{
			if (OnMapUpdate != null) OnMapUpdate(newMap);
			Map = newMap;
		}

		private void AddMove(RobotMove move)
		{
			if (OnMoveAdded != null) OnMoveAdded(move);
		}
	}
}