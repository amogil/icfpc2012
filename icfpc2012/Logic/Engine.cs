using System;
using System.Collections.Generic;

namespace Logic
{
	public class Engine
	{
		public Engine(IMap map)
		{
			Map = map;
		}

		public IMap Map { get; private set; }

		public event Action<IMap> OnMapUpdate;
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
			IMap newMap;
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

		private void UpdateMap(IMap newMap)
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