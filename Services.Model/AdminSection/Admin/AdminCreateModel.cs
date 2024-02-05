using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Services.Model.AdminSection.Admin
{
	/// <summary>
	/// Add new admin
	/// </summary>
	public class AdminCreateModel
	{
		/// <summary>
		/// First name of the admin
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		public string FirstName { get; set; }

	    /// <summary>
	    /// Last name of the admin
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    [Required(AllowEmptyStrings = false)]
	    public string LastName { get; set; }

        /// <summary>
        /// Email of the admin
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required(AllowEmptyStrings = false)]
		[EmailAddress]
		public string Email { get; set; }
	}

    public class AdminCreateProfile : Profile
    {
        public AdminCreateProfile()
        {
           CreateMap<AdminCreateModel, Core.Entities.User>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>true));
        }
    }
}
