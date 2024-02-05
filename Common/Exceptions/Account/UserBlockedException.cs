namespace Common.Exceptions.Account
{
    public class UserBlockedException : ApiException
	{
		public UserBlockedException(string errorMessage)
			: base(errorMessage)
		{

		}
	}
}
