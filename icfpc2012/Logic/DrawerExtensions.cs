using System;
using System.Collections.Generic;
using Logic;

namespace Visualizer
{
	public static class DrawerExtensions
	{
		public static void DrawTarget(this IDrawer drawer, Map map, string style, Tuple<Vector, Stack<RobotMove>> target)
		{
			DrawTarget(drawer, map, map.Robot, style, target);
		}

		public static void DrawTarget(this IDrawer drawer, Map map, Vector from, string style, Tuple<Vector, Stack<RobotMove>> target)
		{
			drawer.Dot(style, target.Item1);
			Vector pos = from;
			foreach (var move in target.Item2)
			{
				Vector pos2 = pos.Add(move.ToVector());
				drawer.Line(style, pos, pos2);
				pos = map.GetTrampolineTarget(pos2);
			}
		}
	}
}