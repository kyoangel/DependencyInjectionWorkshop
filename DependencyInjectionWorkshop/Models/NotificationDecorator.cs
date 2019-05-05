using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class NotificationDecorator : AuthenticationBaseDecorator
	{
		private readonly INotification _notification;

		public NotificationDecorator(IAuthenticationService authentication, INotification notification) : base(authentication)
		{
			_notification = notification;
		}

		private void NotifyUser()
		{
			_notification.PostMessage("my message");
		}

		public override bool Verify(string accountId, string password, string otp)
		{
			var isValid = base.Verify(accountId, password, otp);

			if (!isValid)
			{
				NotifyUser();
			}

			return isValid;
		}
	}
}