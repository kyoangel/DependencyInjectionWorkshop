using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
	public class FailedCounter
	{
		public void IsLock(string accountId)
		{
			var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
			isLockedResponse.EnsureSuccessStatusCode();
			if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
			{
				throw new FailedTooManyTimesException();
			}
		}

		public void ResetFailedCount(string accountId)
		{
			var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
			resetResponse.EnsureSuccessStatusCode();
		}

		public void AddFailedCount(string accountId)
		{
			var addFailedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
			addFailedResponse.EnsureSuccessStatusCode();
		}

		public int GetFailedCount(string accountId)
		{
			var getFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
			getFailedCountResponse.EnsureSuccessStatusCode();
			var failedCount = getFailedCountResponse.Content.ReadAsAsync<int>().Result;
			return failedCount;
		}

		public void LogFailedCountInfo(string accountId, int failedCount)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info($"AccountId:{accountId}, FailedCount:{failedCount}");
		}
	}
}