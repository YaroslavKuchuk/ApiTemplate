using Common.SettingsReflector;

namespace Common.Settings.Invite
{
    public class InviteSettings
    {
        [SettingsProperty("AppStoreLink")]
        public string AppStoreLink { get; set; }
        [SettingsProperty("InviteLink")]
        public string ShareLink { get; set; }
    }
}
