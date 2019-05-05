namespace DependencyInjectionWorkshop.Interface
{
	public interface IAuthentication
	{
		bool Verify(string accountId, string password, string otp);
	}
}