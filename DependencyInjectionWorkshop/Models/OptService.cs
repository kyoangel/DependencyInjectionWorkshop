using System;
using System.Net.Http;
using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class OptService : IOtp
	{
		public string Get(string accountId)
		{
			var apiResponse = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") }.PostAsJsonAsync("api/otps", accountId).Result;
			string currentOtp;
			if (apiResponse.IsSuccessStatusCode)
			{
				currentOtp = apiResponse.Content.ReadAsAsync<string>().Result;
			}
			else
			{
				throw new System.Exception($"web api error, accountId:{accountId}");
			}

			return currentOtp;
		}
	}
}