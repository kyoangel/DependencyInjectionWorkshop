using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Interface;
using DependencyInjectionWorkshop.Repository;
using System;

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

		private void ResetFailedCount(string accountId)
		{
			if (_failedCounter.EnsureUserNotLocked(accountId))
			{
				throw new FailedTooManyTimesException();
			}
		}

		public bool Verify(string accountId, string password, string otp)
		{
			ResetFailedCount(accountId);
			var isValid = _authenticationService.Verify(accountId, password, otp);
			return isValid;
		}
	}

	public class AuthenticationService : IAuthenticationService
	{
		private readonly IFailedCounter _failedCounter;
		private readonly IHash _hash;
		private readonly ILogger _logger;
		private readonly IOtp _optService;
		private readonly IProfile _profile;

		public AuthenticationService()
		{
			_failedCounter = new FailedCounter();
			_profile = new ProfileRepo();
			_hash = new Sha256Adapter();
			_optService = new OptService();
			_logger = new NLogAdapter();
		}

		public AuthenticationService(IFailedCounter failedCounter, IProfile profile, IHash hash, IOtp optService,
			ILogger logger)
		{
			_failedCounter = failedCounter;
			_profile = profile;
			_hash = hash;
			_optService = optService;
			_logger = logger;
		}

		public bool Verify(string accountId, string password, string otp)
		{
			//_failedCountDecorator.ResetFailedCount(accountId);

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

			return false;
		}
	}
}