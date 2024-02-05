using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class NotificationSettings
	{
	    [SettingsProperty("NotificationsSendThreads")]
        public long NotificationsSendThreads { get; set; } = 8;

	    [SettingsProperty("NotificationsSendInterval")]
		public long NotificationsSendInterval { get; set; }

		[SettingsProperty("PendingMessagesSendInterval")]
		public long PendingMessagesSendInterval { get; set; }

		public SmtpServerSettings SmtpSettings { get; set; }

		public SmtpServerCredentials SmtpCredentials { get; set; }

		public PushNotificationSettings PushSettings { get; set; }
		public SmsNotificationSettings SmsSettings { get; set; }
	}
}
