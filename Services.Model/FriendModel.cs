using AutoMapper;
using Services.Model.User;

namespace Services.Model
{
    public class FriendModel : BriefUserModel
    {
        public bool IsFriend { get; set; }
        public bool IsFriendRequestSended { get; set; }
    }

    public class FriendProfile : Profile
    {
        public FriendProfile()
        {
            CreateMap<Core.Entities.User, FriendModel>();
        }
    }
}
