using System;
using System.Text.RegularExpressions;

namespace ValidatorClient
{
	internal static class ResponseParser
	{
		public static ValidatorResponse Parse(string content)
		{
			var match = Regex.Match(content, @"<pre>(?<map>.*?)</pre>\s*<br>\s*Score:\s*(?<score>\d*)\s*<br>(?:(?<result>.*?)<br>)?\s*<h3>High scores</h3>", RegexOptions.Singleline);
			if(!match.Success)
				throw new FormatException(string.Format("Invalid response: '{0}'", content));
			
			int score;

			if(!int.TryParse(match.Groups["score"].Value, out score))
				throw new FormatException(string.Format("Invalid response: score '{0}'", match.Groups["score"].Value));

			return new ValidatorResponse {Map = match.Groups["map"].Value.Trim(), Score = score, Result = match.Groups["result"].Value.Trim()};
		}
	}
}