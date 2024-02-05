namespace Common.Exceptions.Account
{
	public class IncorrectPasswordException : ApiException
	{
		public IncorrectPasswordException(string errorMessage) 
			: base(errorMessage)
		{
		}
	}
}
