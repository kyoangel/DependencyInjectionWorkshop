namespace DependencyInjectionWorkshop.Interface
{
	public interface IFailedCounter
	{
		bool EnsureUserNotLocked(string accountId);
		void Reset(string accountId);
		void Add(string accountId);
		int Get(string accountId);
	}
}