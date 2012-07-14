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

		public void RunProgram(IEnumerable<RobotMove> moves)
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
			Map newMap;
			try
			{
				newMap = Map.Move(robotMove);
				if (newMap.State != CheckResult.Nothing)
					throw new GameFinishedException();
			}
			catch (GameFinishedException)
			{
				AddMove(robotMove);
				UpdateMap(Map);
				throw;
			}
			catch (NoMoveException)
			{
				AddMove(robotMove);
				UpdateMap(Map);
				return;
			}
			AddMove(robotMove);
			UpdateMap(newMap);
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