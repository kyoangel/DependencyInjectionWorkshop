﻿using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
	public class OptService
	{
		public string GetCurrentOtp(string accountId)
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
	}
}