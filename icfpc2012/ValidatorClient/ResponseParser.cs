using System;
using System.Text.RegularExpressions;
using Logic;

namespace ValidatorClient
{
	internal static class ResponseParser
	{
		public static ValidatorResponse Parse(string content)
		{
			var match = Regex.Match(content, @"<pre>(?<map>.*?)</pre>\s*<br>\s*Score:\s*(?<score>-?\d*)\s*<br>(?:(?<result>.*?)<br>)?\s*<h3>High scores</h3>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if(!match.Success)
				throw new FormatException(string.Format("Invalid response: '{0}'", content));
			
			int score;

			if(!int.TryParse(match.Groups["score"].Value, out score))
				throw new FormatException(string.Format("Invalid response: score '{0}'", match.Groups["score"].Value));

			CheckResult checkResult;
			var result = match.Groups["result"].Value.Trim().ToLower();

			switch(result)
			{
				case "":
					checkResult = CheckResult.Nothing;
					break;
				case "robot broken":
					checkResult = CheckResult.Fail;
					break;
				case "mining complete":
					checkResult = CheckResult.Win;
					break;
				default:
					throw new Exception(string.Format("Invalid response: unknown result '{0}'", result));
			}

			return new ValidatorResponse {Map = match.Groups["map"].Value, Score = score, Result = checkResult};
		}
	}
}