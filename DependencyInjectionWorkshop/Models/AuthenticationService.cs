using Dapper;
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

			if (!passwordFromDb.Equals(hash.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
			var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
			string result;
			if (response.IsSuccessStatusCode)
			{
				result = response.Content.ReadAsAsync<string>().Result;
			}
			else
			{
				throw new Exception($"web api error, accountId:{accountId}");
			}

			if (!result.Equals(otp, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			return true;
		}
	}
}