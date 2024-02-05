using System.Net;
using Common.SettingsReflector;

namespace Common.Settings.NotificationSettings
{
	public class SmtpServerCredentials
	{
		[SettingsProperty("SmtpUseDefaultCredentials")]
		public bool UseDefaultCredential { get; set; }

		[SettingsProperty("SmtpServerLogin")]
		public string UserName { get; set; }

		[SettingsProperty("SmtpServerPass")]
		public string Password { get; set; }

		[SettingsProperty("SmtpNotificationEmail")]
		public string SmtpNotificationEmail { get; set; }

		[SettingsProperty("SmtpSenderName")]
		public string SmtpSenderName { get; set; }

		public NetworkCredential GetNetworkCredentials()
		{
			return new NetworkCredential(UserName, Password);
		}
	}
}
