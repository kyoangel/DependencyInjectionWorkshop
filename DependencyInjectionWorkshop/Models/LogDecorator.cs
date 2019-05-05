using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class LogDecorator : AuthenticationBaseDecorator
	{
		private readonly ILogger _logger;
		private readonly IFailedCounter _failedCounter;

		public LogDecorator(IAuthentication authenticationService, ILogger logger, IFailedCounter failedCounter) : base(authenticationService)
		{
			_logger = logger;
			_failedCounter = failedCounter;
		}

		private void LogMessage(string accountId)
		{
			var failedCount = _failedCounter.Get(accountId);

			_logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}

		public override bool Verify(string accountId, string password, string otp)
		{
			var isValid = base.Verify(accountId, password, otp);
			if (!isValid)
			{
				LogMessage(accountId);
			}
			return isValid;
		}
	}
}