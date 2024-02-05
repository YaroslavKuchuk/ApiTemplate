using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
    public class ResendNotificationsSetting
    {
        [SettingsProperty("ResendNotificationsMaxTryCount")]
        public int ResendNotificationsMaxTryCount { get; set; }
    }
}
