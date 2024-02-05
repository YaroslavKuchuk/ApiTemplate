using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Model.Notifications;

namespace Services.Interfaces
{
    public interface IMessageHistoryService
    {
        Task ResendNotifications();

        Task<List<NotificationModel>> GetNotifications(long userId);
        Task DeleteNotification(long notificationId);
        Task MarkNotificationsAsRead(long userId, string ids);
    }
}
