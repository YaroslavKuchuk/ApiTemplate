using Common.SettingsReflector;

namespace Common.Settings
{
    public class InstagramSettings
    {
        [SettingsProperty("InstagramClientId")]
        public string ClientId { get; set; }

        [SettingsProperty("InstagramClientSecret")]
        public string ClientSecret { get; set; }

        [SettingsProperty("InstagramRedirectUrl")]
        public string RedirectUrl { get; set; }
    }
}
