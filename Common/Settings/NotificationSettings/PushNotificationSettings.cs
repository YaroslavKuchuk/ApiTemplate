using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class PushNotificationSettings
	{
	    [SettingsProperty("PushAndroidServerKey")]
		public string AndroidServerKey { get; set; }

		[SettingsProperty("PushAppleCertificatePath")]
		public string AppleCertificatePath { get; set; }

		[SettingsProperty("PushAppleCertificatePassword")]
		public string AppleCertificatePassword { get; set; }

		[SettingsProperty("PushIsProductAppleCertificate")]
		public bool IsProductAppleCertificate { get; set; } = true;

	    [SettingsProperty("MaxNotificationRequests", DefaultValue = "1")]
        public int MaxNotificationRequests { get; set; }
    }
}
