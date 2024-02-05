using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums.Notification;

namespace Core.Entities
{
    public class MessageHistory : BaseEntity
    {
        public long? UserId { get; set; }
        public virtual User User { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }
        public string Message { get; set; }
        public string FormattedMessage { get; set; }
        public string Title { get; set; }
        public NotificationType Type { get; set; }
        public ActivityType ActivityType { get; set; }
        public string MessageId { get; set; }
        public short TryCount { get; set; }
        public NotificationStatus Status { get; set; }
        public string ErrorDescription { get; set; }
        public string Payload { get; set; }
        public string EventCode { get; set; }
        public bool IsRead { get; set; }
    }
}
