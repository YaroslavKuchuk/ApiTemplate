using Common.SettingsReflector;

namespace Common.Settings
{
    public class AWSSESSettings
    {
        [SettingsProperty("AppEmail")]
        public string AppEmail { get; set; }

        [SettingsProperty("Charset")]
        public string Charset { get; set; }

        [SettingsProperty("Region")]
        public string Region { get; set; }

        [SettingsProperty("AmazonAccessKeyId")]
        public string AccessKey { get; set; }

        [SettingsProperty("AmazonSecretAccessKey")]
        public string SecretKey { get; set; }
    }
}
