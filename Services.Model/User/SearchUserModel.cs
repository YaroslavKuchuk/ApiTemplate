using AutoMapper;

namespace Services.Model.User
{
    public class SearchUserModel : BriefUserModel
    {
    }

    public class SearchUserProfile : Profile
    {
        public SearchUserProfile()
        {
            CreateMap<Core.Entities.User, SearchUserModel>();
        }
    }
}
