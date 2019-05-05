namespace DependencyInjectionWorkshop.Interface
{
	public interface IAuthenticationService
	{
		bool Verify(string accountId, string password, string otp);
	}
}