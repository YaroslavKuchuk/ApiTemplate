using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class SmtpServerSettings
	{
		[SettingsProperty("SmtpServerAddress")]
		public string ServerName { get; set; }

		[SettingsProperty("SmtpServerPort")]
		public int ServerPort { get; set; }

		[SettingsProperty("SmtpUseSSL")]
		public bool UseSSL { get; set; }
	}
}
