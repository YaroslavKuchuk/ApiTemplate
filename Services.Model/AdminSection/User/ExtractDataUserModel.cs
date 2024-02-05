using AutoMapper;
using System.Collections.Generic;

namespace Services.Model.AdminSection.User
{
    public class ExtractDataUserModel
    {
        public List<ExtractDataUserItemModel> Users { get; set; }
        public List<ExtractDataUserItemModel> Admins { get; set; }
    }

    public class ExtractDataUserItemModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ExtractDataUserProfile : Profile
    {
        public ExtractDataUserProfile()
        {
            CreateMap<Core.Entities.User, ExtractDataUserModel>();
        }
    }
}