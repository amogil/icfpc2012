using System;
using System.Collections.Generic;
using System.Drawing;
using Logic;

namespace Visualizer
{
	public class ToLambdasOverlay : IOverlay
	{
		public void Draw(Map map, IDrawer drawer)
		{
			var waveRun = new WaveRun(map, map.Robot);
			var first = true;
			drawer.AddStyle("target", "Gold");
			drawer.AddStyle("firstTarget", "Fuchsia");
			Tuple<Vector, Stack<RobotMove>> firstTarget = null;
			foreach (var target in waveRun.EnumerateTargets((lmap, pos, stepNumber) => lmap.GetCell(pos) == MapCell.Lambda))
			{
				if (first) firstTarget = target;
				var style = first ? "firstTarget" : "target";
				first = false;
				drawer.DrawTarget(map, style, target);
			}
			if (waveRun.Lift != null)
			{
				drawer.DrawTarget(map, "target", waveRun.Lift);
			}
			if (firstTarget != null)
				drawer.DrawTarget(map, "firstTarget", firstTarget);
		}


	}
}