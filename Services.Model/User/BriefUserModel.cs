using AutoMapper;

namespace Services.Model.User
{
    public class BriefUserModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
    }

    public class BriefUserProfile : Profile
    {
        public BriefUserProfile()
        {
            CreateMap<Core.Entities.User, BriefUserModel>()
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.Image));
        }
    }
}
