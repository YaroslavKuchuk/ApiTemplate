using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class FacebookSettings
	{
		[SettingsProperty("FacebookAppId")]
		public string ApplicationId { get; set; }

		[SettingsProperty("FacebookSecret")]
		public string SecretKey { get; set; }
	}
}
