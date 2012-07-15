using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logic;
using NUnit.Framework;

namespace Tests
{
	public class ReferenceTestItem
	{
		public ReferenceTestItem(string mapName, string filename, RobotMove[] moves, int score, CheckResult result, string finalMapState)
		{
			MapName = mapName;
			Filename = filename;
			Moves = moves;
			Score = score;
			Result = result;
			FinalMapState = finalMapState;
		}

		public string MapName { get; set; }
		public string Filename { get; set; }
		public RobotMove[] Moves { get; private set; }
		public int Score { get; private set; }
		public CheckResult Result { get; private set; }
		public string FinalMapState { get; private set; }

		public override string ToString()
		{
			return string.Format("test case: {4}\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n", Moves.Aggregate(string.Empty, (s, m) => s + m.ToChar()), Result, Score, FinalMapState, MapName);
		}

		public bool AssertEngineState(Engine e)
		{
			try
			{
				var actualMap = e.Map;
				CheckResult checkResult = actualMap.State;
				if (checkResult == CheckResult.Abort) checkResult = CheckResult.Nothing; //Специфика загружалки результатов валидатора
				Assert.AreEqual(Result, checkResult, this.ToString());
				Assert.AreEqual(Score, actualMap.GetScore(), this.ToString());
				var mapStateAsAscii = GetMapStateAsAscii(actualMap);
				string actualMap1 = Regex.Replace(mapStateAsAscii, "[A-I]", "T"); //Специфика вывода валидатора
				string actualMap2 = Regex.Replace(actualMap1, "[1-9]", "t"); //Специфика вывода валидатора
				Assert.AreEqual(FinalMapState, actualMap2, string.Format("{0}\r\nactual map state:\r\n{1}", this.ToString(), actualMap2));
				return true;
			}
			catch (AssertionException ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public string GetMapStateAsAscii(Map map)
		{
			return new MapSerializer().SerializeMapOnly(map.SkipBorder()).ToString();
		}
	}

	[TestFixture]
	public class Correctness
	{
		private const string mapsBaseDir = @"..\..\..\..\maps";

		private static IEnumerable<Tuple<string, string[]>> GetReferenceMaps()
		{
			return Directory
				.EnumerateFiles(mapsBaseDir, "*.txt")
				.Select(file => Tuple.Create(GetMapName(file), File.ReadAllLines(file)));
		}

		private static string GetMapName(string file)
		{
			var name = new FileInfo(file).Name;
			return name.Substring(0, name.IndexOf(".txt", StringComparison.OrdinalIgnoreCase));
		}

		private static IEnumerable<ReferenceTestItem> GetReferenceTestItems(string mapName)
		{
			return Directory
				.EnumerateFiles(mapsBaseDir, mapName + "*.ref")
				.SelectMany(f => GetReferenceTestItems(mapName, f));
		}

		private static IEnumerable<ReferenceTestItem> GetReferenceTestItems(string mapName, string refFile)
		{
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
					var result = (CheckResult)Enum.Parse(typeof(CheckResult), line.Split(new[] { "Result: " }, StringSplitOptions.RemoveEmptyEntries)[0]);
					line = r.ReadLine();
					var score = int.Parse(line.Split(new[] {"Score: "}, StringSplitOptions.RemoveEmptyEntries)[0]);
					var sb = new StringBuilder();
					while (!string.IsNullOrEmpty(line = r.ReadLine()))
						sb.AppendLine(line);
					yield return new ReferenceTestItem(mapName, refFile, moves, score, result, sb.ToString());
				}
			}
		}

		[Test]
		public void AgainstValidator()
		{
			var allTestsPassed = true;
			foreach (var t in GetReferenceMaps())
			{
				var mapName = t.Item1;
				foreach (var testItem in GetReferenceTestItems(mapName))
				{
					Console.WriteLine(testItem.Filename);
					var engine = new Engine(new Map(t.Item2));
					engine.RunProgram(testItem.Moves);
					allTestsPassed &= testItem.AssertEngineState(engine);
				}
			}
			Assert.IsTrue(allTestsPassed);
		}
		[Test]
		public void AgainstValidatorWithRollbacks()
		{
			var allTestsPassed = true;
			foreach (var t in GetReferenceMaps())
			{
				var mapName = t.Item1;
				foreach (var testItem in GetReferenceTestItems(mapName))
				{
					Console.WriteLine(testItem.Filename);
					var engine = new RollbackEngine(new Map(t.Item2));
					engine.RunProgram(testItem.Moves);
					allTestsPassed &= testItem.AssertEngineState(engine);
				}
			}
			Assert.IsTrue(allTestsPassed);
		}
	}

	public class RollbackEngine : Engine
	{
		public RollbackEngine(Map map) : base(map)
		{
		}
		public override void RunProgram(IEnumerable<RobotMove> moves)
		{
			try
			{
				RobotMove[] robotMoves = moves.ToArray();
				foreach (var move in robotMoves.Take(robotMoves.Length-1))
					DoMove(move);
				foreach (var move in robotMoves.Take(robotMoves.Length - 1))
					Map.Rollback();
				foreach (var move in robotMoves)
					DoMove(move);

			}
			catch (GameFinishedException)
			{
			}
		}
	}
}
