using System;
using AutoMapper;

namespace Services.Model.Account
{
    public class InstagramModel
    {
        public long? Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
    }
    /// <summary>
    /// Create the mappings.
    /// </summary>
    public class InstagramProfile : Profile
    {
        public InstagramProfile()
        {
            CreateMap<InstagramModel, Core.Entities.User>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Username))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Username))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.ProfilePicture))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>false));
        }
    }
}
