using System;
using AutoMapper;

namespace Services.Model.AdminSection.Admin
{
	/// <summary>
	/// Model for list of the admins
	/// </summary>
	public class AdminPreviewModel
	{
		/// <summary>
		/// Admin Id
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public long Id { get; set; }

		/// <summary>
		///  Email of the admin
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		public string Email { get; set; }

		/// <summary>
		/// First name of the admin
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		public string FirstName { get; set; }

	    /// <summary>
	    /// Last name of the admin
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    public string LastName { get; set; }

	    public bool IsActive { get; set; }
	    public DateTime CreateDate { get; set; }

    }
    public class AdminPreviewProfile : Profile
    {
        public AdminPreviewProfile()
        {
            CreateMap<Core.Entities.User, AdminPreviewModel>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName));

            CreateMap<Core.Entities.User, AdminPreviewModel>();
        }
    }
}
