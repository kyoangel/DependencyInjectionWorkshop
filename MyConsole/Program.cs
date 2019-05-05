using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsole
{
	class Program
    {
        static void Main(string[] args)
        {
	        var authentication = SimpleFactory.GetAuthenticationService();
	        var isValid = authentication;

			if (isValid)
			{
				
			}
        }
    }
}
