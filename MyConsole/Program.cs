﻿using Autofac;
using DependencyInjectionWorkshop.Interface;
using DependencyInjectionWorkshop.Models;
using System;

namespace MyConsole
{
	internal class Program
	{
		private static IContainer _container;

		private static void Main(string[] args)
		{
			RegisterContainer();

			var authentication = _container.Resolve<IAuthentication>();
			var isValid = authentication.Verify("joey", "pw", "123457");

			Console.WriteLine(isValid);
		}

		private static void RegisterContainer()
		{
			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterType<FakeOtp>()
				.As<IOtp>();

			containerBuilder.RegisterType<FakeHash>()
				.As<IHash>();

			containerBuilder.RegisterType<FakeProfile>()
				.As<IProfile>();

			containerBuilder.RegisterType<FakeSlack>()
				.As<INotification>();

			containerBuilder.RegisterType<FakeLogger>()
				.As<ILogger>();

			containerBuilder.RegisterType<FakeFailedCounter>()
				.As<IFailedCounter>();

			containerBuilder.RegisterType<LogDecorator>();
			containerBuilder.RegisterType<NotificationDecorator>();
			containerBuilder.RegisterType<FailedCountDecorator>();

			containerBuilder.RegisterType<AuthenticationService>().As<IAuthentication>();

			containerBuilder.RegisterDecorator<NotificationDecorator, IAuthentication>();
			containerBuilder.RegisterDecorator<FailedCountDecorator, IAuthentication>();
			containerBuilder.RegisterDecorator<LogDecorator, IAuthentication>();

			_container = containerBuilder.Build();
		}
	}

	internal class FakeLogger : ILogger
	{
		public void Info(string message)
		{
			Console.WriteLine($"{nameof(LogDecorator)}.{nameof(Info)}({message})");
		}
	}

	internal class FakeSlack : INotification
	{
		public void PostMessage(string message)
		{
			Console.WriteLine($"{nameof(FakeSlack)}.{nameof(PostMessage)}({message})");
		}
	}

	internal class FakeFailedCounter : IFailedCounter
	{
		public bool EnsureUserNotLocked(string accountId)
		{
			Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(EnsureUserNotLocked)}({accountId})");
			return false;
		}

		public void Reset(string accountId)
		{
			Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Reset)}({accountId})");
		}

		public void Add(string accountId)
		{
			Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Add)}({accountId})");
		}

		public int Get(string accountId)
		{
			Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Get)}({accountId})");
			return 91;
		}
	}

	internal class FakeOtp : IOtp
	{
		public string Get(string accountId)
		{
			Console.WriteLine($"{nameof(FakeOtp)}.{nameof(Get)}({accountId})");
			return "123456";
		}
	}

	internal class FakeHash : IHash
	{
		public string GetHash(string plainText)
		{
			Console.WriteLine($"{nameof(FakeHash)}.{nameof(GetHash)}({plainText})");
			return "my hashed password";
		}
	}

	internal class FakeProfile : IProfile
	{
		public string GetPassword(string accountId)
		{
			Console.WriteLine($"{nameof(FakeProfile)}.{nameof(GetPassword)}({accountId})");
			return "my hashed password";
		}
	}
}