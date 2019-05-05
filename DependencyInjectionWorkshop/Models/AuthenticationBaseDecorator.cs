namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationBaseDecorator : IAuthenticationService
	{
		private readonly IAuthenticationService _authentication;

		public AuthenticationBaseDecorator(IAuthenticationService authentication)
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