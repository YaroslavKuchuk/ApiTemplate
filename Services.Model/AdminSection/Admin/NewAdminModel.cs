using System;
using AutoMapper;

namespace Services.Model.AdminSection.Admin
{
    public class NewAdminModel : AdminCreateModel
    {
        /// <summary>
        /// Admin password
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string Password { get; set; }
    }

    public class NewAdminProfile : Profile
    {
        public NewAdminProfile()
        {
            CreateMap<NewAdminModel, Core.Entities.User>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsDelete, opt => opt.MapFrom(src => false))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>true));
        }
    }
}
