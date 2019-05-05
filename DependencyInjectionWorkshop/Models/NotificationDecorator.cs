using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class NotificationDecorator :IAuthenticationService
	{
		private readonly IAuthenticationService _authentication;
		private readonly INotification _notification;

		public NotificationDecorator(IAuthenticationService authentication, INotification notification)
		{
			_authentication = authentication;
			_notification = notification;
		}

		private void NotifyUser()
		{
			_notification.PostMessage("my message");
		}

		public bool Verify(string accountId, string password, string otp)
		{
			var isValid = _authentication.Verify(accountId,password,otp);

			if (!isValid)
			{
				NotifyUser();
			}

			return isValid;
		}
	}
}