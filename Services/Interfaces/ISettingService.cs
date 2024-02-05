using System.Threading.Tasks;
using Common.Settings.NotificationSettings;
using Services.Model.Invite;

namespace Services.Interfaces
{
	public interface ISettingService
	{
		Task<NotificationSettings> GetNotificationServiceSettings();
		Task<T> GetSettings<T>() where T : new();
	    Task<InviteModel> GetInviteSettings(long userId);
	    Task<InviteModel> GetInvitePage(string cipherUserId);
    }
}