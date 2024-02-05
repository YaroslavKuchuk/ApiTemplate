using System.Net;

namespace Common.Exceptions.Account
{
    public class PermissionDeniedExeption : ApiException
    {
        public PermissionDeniedExeption(string errorMessage)
            : base(errorMessage, HttpStatusCode.Forbidden)
        {

        }
    }
}
