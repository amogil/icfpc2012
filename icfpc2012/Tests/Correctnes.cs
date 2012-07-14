using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Logic;
using NUnit.Framework;

namespace Tests
{
	public class ReferenceTestItem
	{
		public ReferenceTestItem(RobotMove[] moves, int score, CheckResult result, string finalMapState)
		{
			Moves = moves;
			Score = score;
			Result = result;
			FinalMapState = finalMapState;
		}

		public RobotMove[] Moves { get; private set; }
		public int Score { get; private set; }
		public CheckResult Result { get; private set; }
		public string FinalMapState { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n", Moves.Aggregate(string.Empty, (s, m) => s + m.ToChar()), Score, Result, FinalMapState);
		}
	}

	[TestFixture]
	public class Correctnes
	{
		private const string mapsBaseDir = @"..\..\..\..\maps";

		private static IEnumerable<Tuple<string, Map>> GetReferenceMaps()
		{
			return Directory
				.EnumerateFiles(mapsBaseDir, "*.txt")
				.Select(file => Tuple.Create(GetMapName(file), new Map(File.ReadAllLines(file))));
		}

		private static string GetMapName(string file)
		{
			var name = new FileInfo(file).Name;
			return name.Substring(0, name.IndexOf(".txt", StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<ReferenceTestItem> GetReferenceTestItems(string mapName)
		{
			var refFile = Path.Combine(mapsBaseDir, mapName + ".ref");
			if (!File.Exists(refFile)) yield break;
			using (var r = new StreamReader(refFile))
			{
				while (true)
				{
					var line = string.Empty;
					while (string.IsNullOrEmpty(line))
					{
						line = r.ReadLine();
						if (line == null) yield break;
					}
					var moves = line.Select(c => c.ToRobotMove()).ToArray();
					line = r.ReadLine();
					var score = int.Parse(line.Split(new[] {"Score: "}, StringSplitOptions.RemoveEmptyEntries)[0]);
					line = r.ReadLine();
					var result = (CheckResult)Enum.Parse(typeof (CheckResult), line.Split(new[] {"Result: "}, StringSplitOptions.RemoveEmptyEntries)[0]);
					var sb = new StringBuilder();
					while (!string.IsNullOrEmpty(line = r.ReadLine()))
						sb.AppendLine(line);
					yield return new ReferenceTestItem(moves, score, result, sb.ToString());
				}
			}
		}

		[Test]
		public void AgainstValidator()
		{
			foreach (var t in GetReferenceMaps())
			{
				var mapName = t.Item1;
				Console.WriteLine(mapName);
				foreach (var testItem in GetReferenceTestItems(mapName))
				{
					Console.WriteLine(testItem);
				}
				
			}
		}
	}
}
