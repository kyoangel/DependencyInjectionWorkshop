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
			HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.dev/") };
			var isLockedResponse = httpClient.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
			isLockedResponse.EnsureSuccessStatusCode();
			if (isLockedResponse.Content.ReadAsAsync<bool>().Result)
			{
				throw new FailedTooManyTimesException();
			}

			var passwordFromDb = string.Empty;
			using (var connection = new SqlConnection("my connection string"))
			{
				passwordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
					commandType: CommandType.StoredProcedure).SingleOrDefault();
			}

			var crypt = new System.Security.Cryptography.SHA256Managed();
			var hash = new StringBuilder();
			var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
			foreach (var theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}

			var apiResponse = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
			string currentOtp;
			if (apiResponse.IsSuccessStatusCode)
			{
				currentOtp = apiResponse.Content.ReadAsAsync<string>().Result;
			}
			else
			{
				throw new Exception($"web api error, accountId:{accountId}");
			}

			var hashedPassword = hash.ToString();
			if (passwordFromDb.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase) &&
				currentOtp.Equals(otp, StringComparison.OrdinalIgnoreCase))
			{
				var resetResponse = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
				resetResponse.EnsureSuccessStatusCode();

				return true;
			}
			else
			{
				var slackClient = new SlackClient("my api token");
				slackClient.PostMessage(response1 => { }, "my channel", "my message", "my bot name");

				var addFailedResponse = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
				addFailedResponse.EnsureSuccessStatusCode();

				return false;
			}
		}
	}
}