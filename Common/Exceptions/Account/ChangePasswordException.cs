namespace Common.Exceptions.Account
{
	public class ChangePasswordException : ApiException
	{
		public ChangePasswordException(string errorMessage) 
			: base(errorMessage)
		{
		}
	}
}
