using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums.Notification;

namespace Services.Interfaces.Notifications
{
    public interface IEmailNotificationService
    {
        Task SendMessageToAdminsAsync(string body, string subject, ActivityType activityType, Dictionary<object, object> payload = null);
        Task SendMessageAsync(string body, string subject, long userId, ActivityType activityType, Dictionary<object, object> payload = null);
        Task SendMessageAsync(string subject, string body, string emailAddress, ActivityType activityType);
        Task ResendMessageAsync(long messageHistoryId);
    }
}
