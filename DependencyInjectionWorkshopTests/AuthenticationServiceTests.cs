﻿using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Interface;
using DependencyInjectionWorkshop.Models;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
	[TestFixture]
	public class AuthenticationServiceTests
	{
		private const string DefaultAccountId = "Kyo";
		private const string DefaultHashedPassword = "1qaz2wsx";
		private const string DefaultOtp = "99999";
		private const string DefaultPassword = "1234qwer";
		private const int DefaultFailedCount = 91;
		private IAuthentication _authentication;
		private IFailedCounter _failedCounter;
		private IHash _hash;
		private ILogger _logger;
		private INotification _notification;
		private IOtp _optService;
		private IProfile _profile;
		private IApiCountQuota _apiCountQuota;

		[SetUp]
		public void Setup()
		{
			_logger = Substitute.For<ILogger>();
			_profile = Substitute.For<IProfile>();
			_optService = Substitute.For<IOtp>();
			_hash = Substitute.For<IHash>();
			_notification = Substitute.For<INotification>();
			_failedCounter = Substitute.For<IFailedCounter>();
			_apiCountQuota = Substitute.For<IApiCountQuota>();

			var authenticationService = new AuthenticationService(_profile, _hash, _optService);
			var notificationDecorator = new NotificationDecorator(authenticationService, _notification);
			var failedCountDecorator = new FailedCountDecorator(notificationDecorator, _failedCounter);
			var logDecorator = new LogDecorator(failedCountDecorator, _logger, _failedCounter);
			var apiCallQuotaDecorator = new ApiCallQuotaDecorator(logDecorator, _apiCountQuota);
			_authentication = apiCallQuotaDecorator;
		}

		[Test]
		public void Add_failedCount_when_user_is_valid()
		{
			WhenInvalid();

			FailedCountShouldAdd();
		}

		[Test]
		public void is_invalid_when_user_locked()
		{
			_failedCounter.When(x => x.EnsureUserNotLocked(Arg.Any<string>()))
				.Do(x => throw new FailedTooManyTimesException());

			Assert.Throws<FailedTooManyTimesException>(() =>
				_authentication.Verify("lock_user", "any_pw", "any_otp"));
		}

		[Test]
		public void is_invalid_when_wrong_otp()
		{
			GivenPassword(DefaultAccountId, DefaultHashedPassword);
			GivenOtp(DefaultAccountId, DefaultOtp);
			GivenHashed(DefaultPassword, DefaultHashedPassword);

			var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
			ShouldBeInvalid(isValid);
		}

		[Test]
		public void is_valid()
		{
			GivenPassword(DefaultAccountId, DefaultHashedPassword);
			GivenOtp(DefaultAccountId, DefaultOtp);
			GivenHashed(DefaultPassword, DefaultHashedPassword);

			var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);

			ShouldBeValid(isValid);
		}

		[Test]
		public void Log_account_failed_count_when_invalid()
		{
			GivenFailedCount(DefaultFailedCount);

			WhenInvalid();

			LogShouldContains(DefaultAccountId, DefaultFailedCount);
		}

		[Test]
		public void Notify_user_when_is_invalid()
		{
			WhenInvalid();

			ShouldNotifyUser();
		}

		[Test]
		public void Reset_failedCount_when_user_is_valid()
		{
			WhenValid();

			FailedCountShouldReset();
		}

		[Test]
		public void Every_apiCall_will_add_api_count()
		{
			WhenValid();

			_apiCountQuota.Received(1).Add(Arg.Any<string>());
		}

		private bool WhenValid()
		{
			GivenPassword(DefaultAccountId, DefaultHashedPassword);
			GivenOtp(DefaultAccountId, DefaultOtp);
			GivenHashed(DefaultPassword, DefaultHashedPassword);

			var isValid = WhenVerify(DefaultAccountId, DefaultPassword, DefaultOtp);
			return isValid;
		}

		private void FailedCountShouldReset()
		{
			_failedCounter.Received(1).Reset(Arg.Any<string>());
		}

		private void FailedCountShouldAdd()
		{
			_failedCounter.Received(1).Add(Arg.Any<string>());
		}

		private static void ShouldBeInvalid(bool isValid)
		{
			Assert.IsFalse(isValid);
		}

		private void LogShouldContains(string account, int failedCount)
		{
			_logger.Received(1).Info(Arg.Is<string>(m => m.Contains(account) && m.Contains(failedCount.ToString())));
		}

		private void GivenFailedCount(int failedCount)
		{
			_failedCounter.Get(Arg.Any<string>()).ReturnsForAnyArgs(failedCount);
		}

		private bool WhenVerify(string defaultAccountId, string defaultPassword, string defaultOtp)
		{
			var isValid = _authentication.Verify(defaultAccountId, defaultPassword, defaultOtp);
			return isValid;
		}

		private static void ShouldBeValid(bool isValid)
		{
			Assert.IsTrue(isValid);
		}

		private void ShouldNotifyUser()
		{
			_notification.Received(1).PostMessage(Arg.Any<string>());
		}

		private bool WhenInvalid()
		{
			GivenPassword(DefaultAccountId, DefaultHashedPassword);
			GivenOtp(DefaultAccountId, DefaultOtp);
			GivenHashed(DefaultPassword, DefaultHashedPassword);

			var isValid = WhenVerify(DefaultAccountId, DefaultPassword, "wrong otp");
			return isValid;
		}

		private void GivenHashed(string defaultPassword, string defaultHashedPassword)
		{
			_hash.GetHash(defaultPassword).ReturnsForAnyArgs(defaultHashedPassword);
		}

		private void GivenOtp(string defaultAccountId, string defaultOtp)
		{
			_optService.Get(defaultAccountId).ReturnsForAnyArgs(defaultOtp);
		}

		private void GivenPassword(string defaultAccountId, string defaultHashedPassword)
		{
			_profile.GetPassword(defaultAccountId).ReturnsForAnyArgs(defaultHashedPassword);
		}
	}
}