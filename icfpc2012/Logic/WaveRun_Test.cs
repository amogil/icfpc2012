using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Logic
{
	[TestFixture]
	public class WaveRun_Test
	{
		[Test]
		public void Test()
		{
			Map map = WellKnownMaps.Contest1();
			var formattedTargets = GetTargets(map.Robot, map);
			Assert.That(formattedTargets, Contains.Item("(4, 4) via DL"));
			Assert.That(formattedTargets, Contains.Item("(2, 3) via DLLDL"));
			formattedTargets = GetTargets(new Vector(2, 2), map);
			Assert.That(formattedTargets, Contains.Item("(2, 3) via U"));
		}

		[Test]
		public void Trampolines()
		{
			Map map = WellKnownMaps.TramTest();
			var formattedTargets = GetTargets(map.Robot, map);
			Assert.That(formattedTargets, Contains.Item("(6, 2) via RR"));
		}

		private static string[] GetTargets(Vector from, Map map)
		{
			Console.WriteLine(map.ToString());
			var waveRun = new WaveRun(map, from);
			Tuple<Vector, Stack<RobotMove>>[] targets = waveRun.EnumerateTargets().ToArray();
			string[] formattedTargets = targets.Select(FormatTarget).ToArray();
			foreach (var target in formattedTargets)
				Console.WriteLine(target);
			return formattedTargets;
		}

		private static string FormatTarget(Tuple<Vector, Stack<RobotMove>> target)
		{
			var commands = new String(target.Item2.Select(m => m.ToChar()).ToArray());
			string format = target.Item1 + " via " + commands;
			return format;
		}
	}
}