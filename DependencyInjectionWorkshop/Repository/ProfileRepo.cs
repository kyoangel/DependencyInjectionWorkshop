﻿using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DependencyInjectionWorkshop.Repository
{
	public class ProfileRepo
	{
		public string GetPasswordFromDb(string accountId)
		{
			var passwordFromDb = string.Empty;
			using (var connection = new SqlConnection("my connection string"))
			{
				passwordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
					commandType: CommandType.StoredProcedure).SingleOrDefault();
			}

			return passwordFromDb;
		}
	}
}