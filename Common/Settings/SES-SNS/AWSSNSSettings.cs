using Common.SettingsReflector;

namespace Common.Settings
{
    public class AWSSNSSettings
    {
        [SettingsProperty("Region")]
        public string Region { get; set; }

        [SettingsProperty("AmazonAccessKeyId")]
        public string AccessKey { get; set; }

        [SettingsProperty("AmazonSecretAccessKey")]
        public string SecretKey { get; set; }

        [SettingsProperty("AmazonIoSEndpointArn")]
        public string AmazonIoSEndpointArn { get; set; }

        [SettingsProperty("AmazonAndroidEndpointArn")]
        public string AmazonAndroidEndpointArn { get; set; }
    }
}
