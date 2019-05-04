namespace DependencyInjectionWorkshop.Adapter
{
	public interface ILogger
	{
		void Info(string accountId, int failedCount);
	}
}