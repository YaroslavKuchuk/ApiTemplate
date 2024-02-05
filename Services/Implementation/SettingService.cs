using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Settings.Invite;
using Common.Settings.NotificationSettings;
using Common.SettingsReflector;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Services.Helpers;
using Services.Interfaces;
using Services.Model.Invite;

namespace Services.Implementation
{
    public class SettingService : ISettingService
	{
		private readonly IRepository<Setting> _settingRepository;
	    private readonly IRepository<User> _userRepository;

        public SettingService(IUnitOfWork unitOfWork)
		{
			_settingRepository = unitOfWork.Repository<Setting>();
		    _userRepository = unitOfWork.Repository<User>();

        }

		public async Task<NotificationSettings> GetNotificationServiceSettings()
		{
			var notificationSettings = await GetSettings<NotificationSettings>();

			notificationSettings.SmtpCredentials = await GetSettings<SmtpServerCredentials>();
			notificationSettings.SmtpSettings = await GetSettings<SmtpServerSettings>();
			notificationSettings.PushSettings = await GetSettings<PushNotificationSettings>();

			return notificationSettings;
		}

		public async Task<T> GetSettings<T>() where T : new()
		{
			var keys = SettingsReflector.GetKeys<T>();
			var settings = await _settingRepository.FindByAsync(x => keys.Contains(x.ParamName.ToLower()));

			var result = settings.ToDictionary(e => e.ParamName, e => e.ParamValue);

			return SettingsReflector.CreateNewObject<T>(result);
		}

	    public async Task<InviteModel> GetInviteSettings(long userId)
	    {
	        var settings = await GetSettings<InviteSettings>();
	        var cipherUserId = Crypto.ConvertStringToHex(userId.ToString());
	        return new InviteModel { AppStoreLink = settings.AppStoreLink, InviteLink = cipherUserId };
	    }

	    public async Task<InviteModel> GetInvitePage(string cipherUserId)
	    {
	        var settings = await GetSettings<InviteSettings>();
	        return new InviteModel
	        {
	            AppStoreLink = settings.AppStoreLink,
	            InviteLink = string.Format(settings.ShareLink, cipherUserId)
	        };
	    }
    }
}
