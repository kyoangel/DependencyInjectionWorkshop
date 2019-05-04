﻿using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Adapter
{
	public class NLogAdapter : ILogger
	{
		public void Info(string message)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();
			logger.Info(message);
		}
	}
}