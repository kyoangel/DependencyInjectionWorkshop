using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Interface;
using DependencyInjectionWorkshop.Repository;

namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationService
	{
		private readonly IFailedCounter _failedCounter;
		private readonly IHash _hash;
		private readonly ILogger _logger;
		private readonly INotification _notification;
		private readonly IOtp _optService;
		private readonly IProfile _profile;

		public AuthenticationService()
		{
			_failedCounter = new FailedCounter();
			_profile = new ProfileRepo();
			_hash = new Sha256Adapter();
			_optService = new OptService();
			_logger = new NLogAdapter();
			_notification = new SlackAdapter();
		}

		public AuthenticationService(IFailedCounter failedCounter, IProfile profile, IHash hash, IOtp optService,
			ILogger logger, INotification notification)
		{
			_failedCounter = failedCounter;
			_profile = profile;
			_hash = hash;
			_optService = optService;
			_logger = logger;
			_notification = notification;
		}

		public bool Verify(string accountId, string password, string otp)
		{
			if (_failedCounter.EnsureUserNotLocked(accountId))
			{
				throw new FailedTooManyTimesException();
			}

			var passwordFromDb = _profile.GetPassword(accountId);

			var hashedPassword = _hash.GetHash(password);

			var currentOtp = _optService.Get(accountId);

			if (passwordFromDb.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase) &&
			    currentOtp.Equals(otp, StringComparison.OrdinalIgnoreCase))
			{
				_failedCounter.Reset(accountId);

				return true;
			}

			_failedCounter.Add(accountId);

			var failedCount = _failedCounter.Get(accountId);

			_logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");

			_notification.PostMessage("my message");

			return false;
		}
	}
}