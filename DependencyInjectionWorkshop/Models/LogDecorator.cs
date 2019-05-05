using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class LogDecorator : IAuthenticationService
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly ILogger _logger;
		private readonly IFailedCounter _failedCounter;

		public LogDecorator(IAuthenticationService authenticationService, ILogger logger, IFailedCounter failedCounter)
		{
			_authenticationService = authenticationService;
			_logger = logger;
			_failedCounter = failedCounter;
		}

		private void LogMessage(string accountId)
		{
			var failedCount = _failedCounter.Get(accountId);

			_logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}

		public bool Verify(string accountId, string password, string otp)
		{
			var isValid = _authenticationService.Verify(accountId, password, otp);
			if (!isValid)
			{
				LogMessage(accountId);
			}
			return isValid;
		}
	}
}