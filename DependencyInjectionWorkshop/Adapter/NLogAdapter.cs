namespace DependencyInjectionWorkshop.Adapter
{
	public class NLogAdapter : ILogger
	{
		public void Info(string accountId, int failedCount)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}
	}
}