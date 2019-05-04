using Dapper;
using SlackAPI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationService
	{
		public bool Verify(string accountId, string password, string otp)
		{
			IsLock(accountId);

			var passwordFromDb = GetPasswordFromDb(accountId);

			var hashedPassword = GetHashedPassword(password);

			var currentOtp = GetCurrentOtp(accountId);

			if (passwordFromDb.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase) &&
				currentOtp.Equals(otp, StringComparison.OrdinalIgnoreCase))
			{
				ResetFailedCount(accountId);

				return true;
			}
			else
			{
				AddFailedCount(accountId);

				var failedCount = GetFailedCount(accountId);

				LogFailedCountInfo(accountId, failedCount);

				Notify();

				return false;
			}
		}

		private static void Notify()
		{
			var slackClient = new SlackClient("my api token");
			slackClient.PostMessage(response1 => { }, "my channel", "my message", "my bot name");
		}

		private static void LogFailedCountInfo(string accountId, int failedCount)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}

		private static int GetFailedCount(string accountId)
		{
			var getFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
			getFailedCountResponse.EnsureSuccessStatusCode();
			var failedCount = getFailedCountResponse.Content.ReadAsAsync<int>().Result;
			return failedCount;
		}

		private static void AddFailedCount(string accountId)
		{
			var addFailedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
			addFailedResponse.EnsureSuccessStatusCode();
		}

		private static void ResetFailedCount(string accountId)
		{
			var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
			resetResponse.EnsureSuccessStatusCode();
		}

		private static string GetCurrentOtp(string accountId)
		{
			var apiResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/otps", accountId).Result;
			string currentOtp;
			if (apiResponse.IsSuccessStatusCode)
			{
				currentOtp = apiResponse.Content.ReadAsAsync<string>().Result;
			}
			else
			{
				throw new Exception($"web api error, accountId:{accountId}");
			}

			return currentOtp;
		}

		private static string GetHashedPassword(string password)
		{
			var crypt = new System.Security.Cryptography.SHA256Managed();
			var hash = new StringBuilder();
			var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
			foreach (var theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}

			var hashedPassword = hash.ToString();
			return hashedPassword;
		}

		private static string GetPasswordFromDb(string accountId)
		{
			var passwordFromDb = string.Empty;
			using (var connection = new SqlConnection("my connection string"))
			{
				passwordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
					commandType: CommandType.StoredProcedure).SingleOrDefault();
			}

			return passwordFromDb;
		}

		private static void IsLock(string accountId)
		{
			var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
			isLockedResponse.EnsureSuccessStatusCode();
			if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
			{
				throw new FailedTooManyTimesException();
			}
		}
	}
}