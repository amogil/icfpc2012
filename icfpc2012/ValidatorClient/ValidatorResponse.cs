using Logic;

namespace ValidatorClient
{
	internal class ValidatorResponse
	{
		public CheckResult Result { get; set; }
		public int Score { get; set; }
		public string Map { get; set; }
	}
}