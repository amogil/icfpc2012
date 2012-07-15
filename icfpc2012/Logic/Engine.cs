using System;
using System.Collections.Generic;

namespace Logic
{
	public class Engine
	{
		public event Action<Map> OnMapUpdate;
		public event Action<RobotMove> OnMoveAdded;

		public virtual Map RunProgram(Map map, IEnumerable<RobotMove> moves)
		{
			foreach (var move in moves)
			{
				map = DoMove(move, map);
				if (map.State != CheckResult.Nothing) break;
			}
			return map;
		}

		public Map DoMove(RobotMove robotMove, Map map)
		{
			if (map.State != CheckResult.Nothing) return map;
			var resMap = map.Move(robotMove);
			AddMove(robotMove);
			UpdateMap(resMap);
			return resMap;
		}

		private void UpdateMap(Map newMap)
		{
			if (OnMapUpdate != null) OnMapUpdate(newMap);
		}

		private void AddMove(RobotMove move)
		{
			if (OnMoveAdded != null) OnMoveAdded(move);
		}
	}
}