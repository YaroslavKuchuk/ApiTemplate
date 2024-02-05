using System;

namespace Core.Entities.Notification
{
    public class AmazonTopic : BaseEntity
    {
        public string Arn { get; set; }
        public string DeviceToken { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
