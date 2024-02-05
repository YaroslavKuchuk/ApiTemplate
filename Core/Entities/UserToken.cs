namespace Core.Entities
{
    public class UserToken : BaseEntity
    {
        public long UserId { get; set; }
        public long? UserDeviceId { get; set; }
        public string AuthToken { get; set; }

        public /*virtual*/ User User { get; set; }
        public /*virtual*/ UserDevice UserDevice { get; set; }
    }
}
