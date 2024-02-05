namespace Common.Exceptions.Account
{
	public class IdentityException : ApiException
	{
		public IdentityException(string errorMessage) 
			: base(errorMessage)
		{
		}
	}
}
