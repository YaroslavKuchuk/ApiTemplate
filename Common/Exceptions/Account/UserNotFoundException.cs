using System.Net;

namespace Common.Exceptions.Account
{
	public class UserNotFoundException : ApiException
	{
		public UserNotFoundException(string errorMessage) 
			: base(errorMessage, HttpStatusCode.NotFound)
		{
		}
	}
}
