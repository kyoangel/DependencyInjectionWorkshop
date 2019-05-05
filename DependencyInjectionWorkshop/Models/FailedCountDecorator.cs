using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class FailedCountDecorator : IAuthenticationService
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IFailedCounter _failedCounter;

		public FailedCountDecorator(IAuthenticationService authenticationService, IFailedCounter failedCounter)
		{
			_authenticationService = authenticationService;
			_failedCounter = failedCounter;
		}

		private void CheckIfUserLocked(string accountId)
		{
			if (_failedCounter.EnsureUserNotLocked(accountId))
			{
				throw new FailedTooManyTimesException();
			}
		}

		public bool Verify(string accountId, string password, string otp)
		{
			CheckIfUserLocked(accountId);
			var isValid = _authenticationService.Verify(accountId, password, otp);
			if (isValid)
			{
				ResetFailedCount(accountId);
			}
			else
			{
				AddFailedCount(accountId);
			}
			return isValid;
		}

		public void AddFailedCount(string accountId)
		{
			_failedCounter.Add(accountId);
		}

		public void ResetFailedCount(string accountId)
		{
			_failedCounter.Reset(accountId);
		}
	}
}