using System.Threading;

namespace Logic
{
	public class TimeAwaredBackTrakingGreedyBot : RobotAI
	{
		private readonly BackTrakingGreedyBot backTrakingGreedyBot = new BackTrakingGreedyBot();
		private readonly int timeLimit;
		private bool isThreadStarted;

		public TimeAwaredBackTrakingGreedyBot()
			:this(15)
		{
			
		}
		public TimeAwaredBackTrakingGreedyBot(int timeLimit)
		{
			this.timeLimit = timeLimit;
		}

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
			           		Thread.Sleep(timeLimit * 1000);
			           		backTrakingGreedyBot.StopNow = true;
			           	}).Start();
		}
	}
}