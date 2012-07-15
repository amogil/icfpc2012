using System.Threading;

namespace Logic
{
	public class TimeAwaredBackTrackingGreedyBot : RobotAI
	{
		private readonly BackTra�kingGreedyBot _backTra�kingGreedyBot = new BackTra�kingGreedyBot();
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
			return _backTra�kingGreedyBot.NextMove(map);
		}

		private void StartThread()
		{
			new Thread(() =>
			           	{
			           		Thread.Sleep(timeLimit * 1000);
			           		_backTra�kingGreedyBot.StopNow = true;
			           	}).Start();
		}
	}
}