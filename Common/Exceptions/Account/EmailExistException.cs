namespace Common.Exceptions.Account
{
	public class EmailExistException : ApiException
	{
		public EmailExistException(string errorMessage) 
			: base(errorMessage)
		{

		}
	}
}