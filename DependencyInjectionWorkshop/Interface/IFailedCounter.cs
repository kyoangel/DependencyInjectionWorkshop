namespace DependencyInjectionWorkshop.Interface
{
	public interface IFailedCounter
	{
		void EnsureUserNotLocked(string accountId);
		void Reset(string accountId);
		void Add(string accountId);
		int Get(string accountId);
	}
}