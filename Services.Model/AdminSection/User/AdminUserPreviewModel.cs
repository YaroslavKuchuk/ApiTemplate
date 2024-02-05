using AutoMapper;

namespace Services.Model.AdminSection.User
{
	/// <summary>
	/// user model for preview at admin panel
	/// </summary>
	public class AdminUserPreviewModel
	{
		/// <summary>
		/// User Id
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public long Id { get; set; }

		/// <summary>
		/// User Email
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		public string Email { get; set; }

		/// <summary>
		/// User first name
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		public string FirstName { get; set; }

	    /// <summary>
	    /// User last name
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    public string LastName { get; set; }

		public bool IsActive { get; set; }


	   
	}
    public class AdminUserPreviewProfile : Profile
    {
        public AdminUserPreviewProfile()
        {
           CreateMap<Core.Entities.User, AdminUserPreviewModel>()
               .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
               .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
               .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
               .ForMember(m => m.IsActive, opt => opt.MapFrom(x => x.IsActive));
        }
    }
}
