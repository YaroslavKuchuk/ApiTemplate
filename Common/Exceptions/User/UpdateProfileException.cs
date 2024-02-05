namespace Common.Exceptions.User
{
	public class UpdateProfileException : ApiException
	{
		public UpdateProfileException(string errorMessage) : base(errorMessage)
		{
		}
	}
}
