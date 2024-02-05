using AutoMapper;
using Newtonsoft.Json;
using System;

namespace Services.Model.AdminSection.ExportImport
{
    public class UserExportModel
    {
        /// <summary>
        /// Admin Id
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("Idintifacator")]
        public long Id { get; set; }

        /// <summary>
        ///  Email of the admin
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [JsonProperty("User's email")]
        public string Email { get; set; }

        /// <summary>
        /// First name of the admin
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [JsonProperty("First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the admin
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [JsonProperty("Last Name")]
        public string LastName { get; set; }
        [JsonProperty("Active")]
        public string IsActive { get; set; }
        [JsonProperty("Create Date")]
        public DateTime CreateDate { get; set; }

    }
    public class UserExportModelProfile : Profile
    {
        public UserExportModelProfile()
        {
            CreateMap<Core.Entities.User, UserExportModel>()
               .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
               .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
               .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
               .ForMember(m => m.IsActive, opt => opt.MapFrom(x => x.IsActive ? "Yes" : "No"));
        }
    }

}
