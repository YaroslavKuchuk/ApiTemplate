
using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class SmsNotificationSettings
	{
		[SettingsProperty("TwilioSid")]
		public string AccountSid { get; set; }

		[SettingsProperty("TwilioToken")]
		public string AuthToken { get; set; }

		[SettingsProperty("TwilioPhone")]
		public string PhoneNumber { get; set; }
	}
}
