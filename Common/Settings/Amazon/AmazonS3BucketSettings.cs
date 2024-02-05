using Common.SettingsReflector;

namespace Common.Settings.Amazon
{
    public class AmazonS3BucketSettings
    {
        [SettingsProperty("AmazonBucketName")]
        public string BucketName { get; set; }

        [SettingsProperty("AmazonCDNUrl")]
        public string CDNUrl { get; set; }

        [SettingsProperty("AmazonAccessKeyId")]
        public string AccessKeyId { get; set; }

        [SettingsProperty("AmazonSecretAccessKey")]
        public string SecretAccessKey { get; set; }

        [SettingsProperty("AmazonsS3Region")]
        public string BucketRegion { get; set; }
    }
}
