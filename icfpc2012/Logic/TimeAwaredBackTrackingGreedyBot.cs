using System.Threading;

namespace Logic
{
	public class TimeAwaredBackTrackingGreedyBot : RobotAI
	{
		private readonly BackTrañkingGreedyBot _backTrañkingGreedyBot = new BackTrañkingGreedyBot();
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
			return _backTrañkingGreedyBot.NextMove(map);
		}

		private void StartThread()
		{
			new Thread(() =>
			           	{
			           		Thread.Sleep(timeLimit * 1000);
			           		_backTrañkingGreedyBot.StopNow = true;
			           	}).Start();
		}
	}
}