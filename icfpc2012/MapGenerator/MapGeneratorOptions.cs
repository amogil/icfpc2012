namespace MapGenerator
{
	public class MapGeneratorOptions
	{
		public MapGeneratorOptions(int height, int width, bool hasLift = true, int rockCount = 0, int earthCount = 0,
		                           int wallCount = 0, int lambdaCount = 1, int waterLevel = 0, int flooding = 0,
		                           int waterproof = 0, int trampolineCount = 0, int beardCount = 0, int beardGrowth = 0,
		                           int mapRazorCount = 0, int pocketRazorCount = 0, int highRockCount = 0)
		{
			Height = height;
			Width = width;
			HasLift = hasLift;
			RockCount = rockCount;
			EarthCount = earthCount;
			WallCount = wallCount;
			LambdaCount = lambdaCount;
			WaterLevel = waterLevel;
			Flooding = flooding;
			Waterproof = waterproof;
			TrampolineCount = trampolineCount;
			BeardCount = beardCount;
			BeardGrowth = beardGrowth;
			MapRazorCount = mapRazorCount;
			PocketRazorCount = pocketRazorCount;
			HighRockCount = highRockCount;
		}

		public int Height { get; private set; }
		public int Width { get; private set; }
		public bool HasLift { get; private set; }
		public int RockCount { get; private set; }
		public int EarthCount { get; private set; }
		public int WallCount { get; private set; }
		public int LambdaCount { get; private set; }
		public int WaterLevel { get; private set; }
		public int Flooding { get; private set; }
		public int Waterproof { get; private set; }
		public int TrampolineCount { get; private set; }
		public int BeardCount { get; private set; }
		public int BeardGrowth { get; private set; }
		public int MapRazorCount { get; private set; }
		public int PocketRazorCount { get; private set; }
		public int HighRockCount { get; private set; }
	}
}