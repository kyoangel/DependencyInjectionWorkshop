using System;
using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class ApiCountQuota : IApiCountQuota
	{
		private int _counter;

		public void Add(string accountId)
		{
			Console.WriteLine($"Api Call times{_counter++} for account {accountId}");
		}
	}
}