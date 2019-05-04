using System.Runtime.Serialization;

namespace DependencyInjectionWorkshop.Exception
{
	public class FailedTooManyTimesException : System.Exception
	{
		public FailedTooManyTimesException()
		{
		}

		public FailedTooManyTimesException(string message) : base(message)
		{
		}

		public FailedTooManyTimesException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		protected FailedTooManyTimesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}