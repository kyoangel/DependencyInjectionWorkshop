using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Models
{
	public class ApiCallQuotaDecorator : AuthenticationBaseDecorator
	{
		private readonly IApiCountQuota _apiCountQuota;

		public ApiCallQuotaDecorator(IAuthentication authenticationService, IApiCountQuota apiCountQuota) : base(authenticationService)
		{
			_apiCountQuota = apiCountQuota;
		}

		public override bool Verify(string accountId, string password, string otp)
		{
			AddApiCallCount(accountId);
			var isValid = base.Verify(accountId, password, otp);
			return isValid;
		}

		private void AddApiCallCount(string accountId)
		{
			_apiCountQuota.Add(accountId);
		}
	}
}