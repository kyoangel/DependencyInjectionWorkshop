using System.Text;
using DependencyInjectionWorkshop.Interface;

namespace DependencyInjectionWorkshop.Adapter
{
	public class Sha256Adapter : IHash
	{
		public string GetHash(string password)
		{
			var crypt = new System.Security.Cryptography.SHA256Managed();
			var hash = new StringBuilder();
			var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
			foreach (var theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}

			var hashedPassword = hash.ToString();
			return hashedPassword;
		}
	}
}