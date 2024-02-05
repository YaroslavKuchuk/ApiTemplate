using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Enums.Notification;
using Services.Model.Messages;

namespace Services.Interfaces.Notifications
{
    public interface IPushNotificationService
    {
        Task SendMessageAsync(string message, string title, long userId, ActivityType activityType,
            string eventCode = null, Dictionary<object, object> payload = null,
            List<FormattedMessageModel> formattedMessage = null);
        Task SendMessageAsync(string message, string title, string phoneNumber, ActivityType activityType,
           string eventCode = null, Dictionary<object, object> payload = null,
           List<FormattedMessageModel> formattedMessage = null);
        Task ResendMessageAsync(long messageHistoryId);
    }
}
