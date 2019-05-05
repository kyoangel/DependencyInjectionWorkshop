using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationBaseDecorator : IAuthentication
	{
		private readonly IAuthentication _authentication;

		public AuthenticationBaseDecorator(IAuthentication authentication)
		{
			_authentication = authentication;
		}

		public virtual bool Verify(string accountId, string password, string otp)
		{
			var isValid = _authentication.Verify(accountId, password, otp);
			return isValid;
		}
	}
}