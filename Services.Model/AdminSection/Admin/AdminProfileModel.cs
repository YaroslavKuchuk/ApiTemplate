using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Services.Model.AdminSection.Admin
{
	public class AdminProfileModel
	{
		/// <summary>
		/// Admin first name
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		public string FirstName { get; set; }

	    /// <summary>
	    /// Admin last name
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    public string LastName { get; set; }

        /// <summary>
        /// Admin email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required(AllowEmptyStrings = false)]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Admin profile image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		public string Image { get; set; }
	    public bool IsActive { get; set; }

    }
    public class AdminProfileProfile : Profile
    {
        public AdminProfileProfile()
        {
           CreateMap<AdminProfileModel, Core.Entities.User>()
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.Image))
               .ForMember(m => m.IsActive, opt => opt.Ignore());

            CreateMap<Core.Entities.User, AdminProfileModel>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.UserName))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.Image, opt => opt.MapFrom(x => x.Image));
        }
    }
}
