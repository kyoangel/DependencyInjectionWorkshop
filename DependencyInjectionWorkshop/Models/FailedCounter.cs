using System;
using System.Net.Http;
using DependencyInjectionWorkshop.Exception;
using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class FailedCounter : IFailedCounter
	{
		public bool EnsureUserNotLocked(string accountId)
		{
			var isLockedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/EnsureUserNotLocked", accountId).Result;
			isLockedResponse.EnsureSuccessStatusCode();
			return isLockedResponse.Content.ReadAsAsync<bool>().Result;
		}

		public void Reset(string accountId)
		{
			var resetResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
			resetResponse.EnsureSuccessStatusCode();
		}

		public void Add(string accountId)
		{
			var addFailedResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
			addFailedResponse.EnsureSuccessStatusCode();
		}

		public int Get(string accountId)
		{
			var getFailedCountResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/failedCounter/Get", accountId).Result;
			getFailedCountResponse.EnsureSuccessStatusCode();
			var failedCount = getFailedCountResponse.Content.ReadAsAsync<int>().Result;
			return failedCount;
		}
	}
}