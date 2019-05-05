using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Interface;
using DependencyInjectionWorkshop.Repository;
using System;

namespace DependencyInjectionWorkshop.Models
{
	public class AuthenticationService : IAuthentication
	{
		private readonly IHash _hash;
		private readonly IOtp _optService;
		private readonly IProfile _profile;

		public AuthenticationService()
		{
			_profile = new ProfileRepo();
			_hash = new Sha256Adapter();
			_optService = new OptService();
		}

		public AuthenticationService(IProfile profile, IHash hash, IOtp optService)
		{
			_profile = profile;
			_hash = hash;
			_optService = optService;
		}

		public bool Verify(string accountId, string password, string otp)
		{
			var passwordFromDb = _profile.GetPassword(accountId);

			var hashedPassword = _hash.GetHash(password);

			var currentOtp = _optService.Get(accountId);

			var isValid = passwordFromDb.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase) &&
						 currentOtp.Equals(otp, StringComparison.OrdinalIgnoreCase);
			return isValid;
		}
	}
}