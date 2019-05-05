using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repository;

namespace MyConsole
{
	class SimpleFactory
	{
		public static bool GetAuthenticationService()
		{
			var authentication = new ApiCallQuotaDecorator(
				new LogDecorator(
					new FailedCountDecorator(
						new NotificationDecorator(
							new AuthenticationService(new ProfileRepo(), new Sha256Adapter(), new OptService()),
							new SlackAdapter()), new FailedCounter()), new NLogAdapter(), new FailedCounter()),
				new ApiCountQuota()).Verify("Kyo", "123456", "999999");
			return authentication;
		}
	}
}