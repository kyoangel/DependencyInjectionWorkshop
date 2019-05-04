using System;

namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationService
	{
		private readonly ProfileRepo _profileRepo = new ProfileRepo();
		private readonly Sha256Adapter _sha256Adapter = new Sha256Adapter();
		private readonly OptService _optService = new OptService();
		private readonly FailedCounter _failedCounter = new FailedCounter();
		private readonly SlackAdapter _slackAdapter = new SlackAdapter();
		private readonly NLogAdapter _nLogAdapter = new NLogAdapter();

		public bool Verify(string accountId, string password, string otp)
		{
			_failedCounter.EnsureUserNotLocked(accountId);

			var passwordFromDb = _profileRepo.GetPasswordFromDb(accountId);

			var hashedPassword = _sha256Adapter.GetHashedPassword(password);

			var currentOtp = _optService.GetCurrentOtp(accountId);

			if (passwordFromDb.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase) &&
				currentOtp.Equals(otp, StringComparison.OrdinalIgnoreCase))
			{
				_failedCounter.ResetFailedCount(accountId);

				return true;
			}
			else
			{
				_failedCounter.AddFailedCount(accountId);

				var failedCount = _failedCounter.GetFailedCount(accountId);

				_nLogAdapter.LogFailedcount(accountId, failedCount);

				_slackAdapter.Notify();

				return false;
			}
		}
	}
}