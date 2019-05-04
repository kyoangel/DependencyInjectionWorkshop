namespace DependencyInjectionWorkshop.Models
{
	public class NLogAdapter
	{
		public void LogFailedcount(string accountId, int failedCount)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}
	}
}