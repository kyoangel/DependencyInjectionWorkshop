namespace DependencyInjectionWorkshop.Interface
{
	public interface IProfile
	{
		string GetPasswordFromDb(string accountId);
	}
}