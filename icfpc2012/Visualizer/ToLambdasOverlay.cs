using System;
using System.Collections.Generic;
using System.Drawing;
using Logic;

namespace Visualizer
{
	public class ToLambdasOverlay : IOverlay
	{
		public void Draw(IMap map, Drawer drawer)
		{
			var waveRun = new WaveRun(map, map.Robot);
			var first = true;
			drawer.AddStyle("target", new Pen(Color.Gold, 1));
			drawer.AddStyle("firstTarget", new Pen(Color.Fuchsia, 3));
			Tuple<Vector, Stack<RobotMove>> firstTarget = null;
			foreach (var target in waveRun.EnumerateTargets())
			{
				if (first) firstTarget = target;
				var style = first ? "firstTarget" : "target";
				first = false;
				DrawTarget(map, drawer, style, target);
			}
			if (firstTarget != null)
				DrawTarget(map, drawer, "firstTarget", firstTarget);
		}

		private static void DrawTarget(IMap map, Drawer drawer, string style, Tuple<Vector, Stack<RobotMove>> target)
		{
			drawer.Dot(style, target.Item1);
			Vector pos = map.Robot;
			foreach (var move in target.Item2)
			{
				Vector pos2 = pos.Add(move.ToVector());
				drawer.Line(style, pos, pos2);
				pos = map.GetTrampolineTarget(pos2);
			}
		}
	}
}