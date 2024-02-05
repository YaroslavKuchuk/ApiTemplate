using System.Net;

namespace Common.Exceptions.User
{
    public class FriendAlreadyAddedException : ApiException
    {
        public FriendAlreadyAddedException(string errorMessage) : base(errorMessage, HttpStatusCode.BadRequest) { }
    }
}
