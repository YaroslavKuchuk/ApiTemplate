using Amazon.Runtime;

namespace Services.Helpers
{
    public class AWSServiceCredentials : AWSCredentials
    {
        private ImmutableCredentials _credentials;
        private readonly string _awsAccessKeyId;
        private readonly string _awsSecretAccessKey;
        private readonly string _token;

        public AWSServiceCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token)
        {
            _awsAccessKeyId = awsAccessKeyId;
            _awsSecretAccessKey = awsSecretAccessKey;
            _token = token;
        }

        public override ImmutableCredentials GetCredentials()
        {
            if (_credentials == null)
            {
                if (_credentials == null)
                {
                    _credentials = new ImmutableCredentials(_awsAccessKeyId, _awsSecretAccessKey, _token);
                }
            }
            return _credentials;
        }
    }
}
