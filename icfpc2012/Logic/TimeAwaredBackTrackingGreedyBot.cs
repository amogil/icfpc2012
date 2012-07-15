using System.Threading;

namespace Logic
{
	public class TimeAwaredBackTrackingGreedyBot : RobotAI
	{
		private readonly BackTrackingGreedyBot backTrackingGreedyBot = new BackTrackingGreedyBot();
		private readonly int timeLimit;
		private bool isThreadStarted;

		public TimeAwaredBackTrackingGreedyBot()
			: this(15)
		{
		}

		public TimeAwaredBackTrackingGreedyBot(int timeLimit)
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
			return backTrackingGreedyBot.NextMove(map);
		}

		private void StartThread()
		{
			var thread = new Thread(() =>
			                        	{
			                        		Thread.Sleep(timeLimit*1000);
			                        		backTrackingGreedyBot.StopNow = true;
			                        	});
			thread.IsBackground = true;
			thread.Start();
		}
	}
}