using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class UserDevice : BaseEntity
    {
        public UserDevice()
        {
            UserTokens = new HashSet<UserToken>();
        }

        public int Status { get; set; }
        public short OsType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string DevicePushToken { get; set; }
        public string DeviceId { get; set; }
        public bool IsEnabledPush { get; set; }

        public /*virtual*/ ICollection<UserToken> UserTokens { get; set; }
    }
}
