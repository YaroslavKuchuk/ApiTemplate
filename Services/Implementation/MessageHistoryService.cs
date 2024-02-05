using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Settings.NotificationSettings;
using Core.Data;
using Core.Data.Repositories;
using Core.Entities;
using Core.Enums.Notification;
using Services.Interfaces;
using Services.Interfaces.Notifications;
using Services.Model.Notifications;

namespace Services.Implementation
{
    public class MessageHistoryService : IMessageHistoryService
    {
        private readonly IRepository<MessageHistory> _messageHistoryRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ResendNotificationsSetting _resendSetting;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHistoryService(IUnitOfWork unitOfWork, ISettingService settingService, IEmailNotificationService emailNotificationService, IPushNotificationService pushNotificationService)
        {
            _unitOfWork = unitOfWork;
            _messageHistoryRepository = unitOfWork.Repository<MessageHistory>();
            _userRepository = _unitOfWork.Repository<User>();
            _resendSetting = settingService.GetSettings<ResendNotificationsSetting>().Result;
            _emailNotificationService = emailNotificationService;
            _pushNotificationService = pushNotificationService;
        }

        public async Task ResendNotifications()
        {
            var failedNotifications = await _messageHistoryRepository.FindByAsync(x => x.Status == NotificationStatus.Failed || x.Status == NotificationStatus.NotSent && x.TryCount < _resendSetting.ResendNotificationsMaxTryCount);
            foreach (var notification in failedNotifications)
            {
                switch (notification.Type)
                {
                    case NotificationType.Email:
                        await _emailNotificationService.ResendMessageAsync(notification.Id);
                        break;
                    case NotificationType.Push:
                        await _pushNotificationService.ResendMessageAsync(notification.Id);
                        break;
                }
            }
        }

        public async Task<List<NotificationModel>> GetNotifications(long userId)
        {
            var allNotifications = await _messageHistoryRepository.FindByAsync(m => m.UserId == userId && m.Status == NotificationStatus.Sent && m.Type == NotificationType.Push);
            var notificationsModel = Mapper.Map<List<NotificationModel>>(allNotifications.OrderByDescending(x=>x.UpdateDate).Take(10));
            return notificationsModel;
        }

        public async Task MarkNotificationsAsRead(long userId, string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return;
            var messagsIds = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var messagsIdsCount = messagsIds.Count;
            if (messagsIdsCount == 0) return;

            var dbName = _unitOfWork.GetDbName();
            var updateCommand = $"update {dbName}.{_unitOfWork.GetTableName<MessageHistory>()} set IsRead=1 where Id IN ({string.Join(",", messagsIds)});";
            await _unitOfWork.ExecuteSqlCommand(updateCommand);
            await UpdateUserUnreadMessageCount(userId, messagsIdsCount);
        }

        private async Task UpdateUserUnreadMessageCount(long userId, int readMessageCount)
        {
            var user = await _userRepository.GetSingleAsync(userId);
            user.UnreadMessageCount = -readMessageCount;
            if (user.UnreadMessageCount < 0)
            {
                user.UnreadMessageCount = 0;
            }
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteNotification(long notificationId)
        {
            var dbName = _unitOfWork.GetDbName();
            var deleteCommand = $"delete from {dbName}.{_unitOfWork.GetTableName<MessageHistory>()} where Id={notificationId}";
            await _unitOfWork.ExecuteSqlCommand(deleteCommand);
        }
    }
}
