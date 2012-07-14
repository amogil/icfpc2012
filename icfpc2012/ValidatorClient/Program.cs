using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ValidatorClient
{
    internal class Program
    {
    	private static void Main(string[] args)
        {
			try
			{
				if(args.Length == 0)
				{
					var random = new Random();
					Directory.SetCurrentDirectory(MapsDir);
					foreach(var filename in Directory.GetFiles(".", "*.moves"))
					{
						try
						{
							if(File.Exists(Path.GetFileNameWithoutExtension(filename) + Extension))
								continue;
							ProcessMovesFile(filename);
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
						Thread.Sleep(600000 + random.Next(60000));
					}
					return;
				}

				var file = args[0];
				Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));

				ProcessMovesFile(file);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
        }

    	private static void ProcessMovesFile(string file)
    	{
			Console.WriteLine("File: {0}", file);

    		var match = Regex.Match(Path.GetFileName(file), @"^(?<map>\w*)\.map_(?<id>.*?).moves$");
    		if (!match.Success)
    			throw new Exception("Invalid filename");

    		var lines = File.ReadAllLines(file);
    		if (lines.Length == 0 || lines[0].Length == 0)
    			throw new Exception("Invalid .moves file format");

    		var moves = lines[0];
    		var map = match.Groups["map"].Value;
    		var id = match.Groups["id"].Value;

    		var response = ResponseParser.Parse(HttpClient.SendRequest(map, moves));
    		SerializeResponse(response, map, id, moves);

			Console.WriteLine(moves);
			Console.WriteLine("Response: score {0}, {1}", response.Score, response.Result);
    	}

    	private static void SerializeResponse(ValidatorResponse response, string map, string id, string moves)
		{
			using(var writer = new StreamWriter(string.Format("{0}.map_{1}.ref", map, id)))
			{
				writer.WriteLine(moves);
				writer.WriteLine("Result: {0}", response.Result);
				writer.WriteLine("Score: {0}", response.Score);
				writer.Write(response.Map);
				writer.WriteLine();
			}
		}

		private const string MapsDir = "../../../../maps";
		private const string Extension = ".ref";
	}
}