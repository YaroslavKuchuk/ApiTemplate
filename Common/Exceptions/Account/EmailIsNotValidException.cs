using System.Net;

namespace Common.Exceptions.Account
{
    public class EmailIsNotValidException : ApiException
    {
        public EmailIsNotValidException(string errorMessage) : base(errorMessage, HttpStatusCode.BadRequest)
        {
        }
    }
}
