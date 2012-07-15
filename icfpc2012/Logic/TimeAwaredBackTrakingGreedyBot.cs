using System.Threading;

namespace Logic
{
	public class TimeAwaredBackTrakingGreedyBot : RobotAI
	{
		private readonly BackTrakingGreedyBot backTrakingGreedyBot = new BackTrakingGreedyBot();
		private bool isThreadStarted;

		public override RobotMove NextMove(Map map)
		{
			if(!isThreadStarted)
			{
				isThreadStarted = true;
				StartThread();
			}
			return backTrakingGreedyBot.NextMove(map);
		}

		private void StartThread()
		{
			new Thread(() =>
			           	{
			           		Thread.Sleep(140 * 1000);
			           		backTrakingGreedyBot.StopNow = true;
			           	}).Start();
		}
	}
}