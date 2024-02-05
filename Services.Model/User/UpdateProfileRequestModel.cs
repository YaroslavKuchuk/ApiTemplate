using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Services.Model.User
{
	/// <summary>
	/// Edit user or admin profile
	/// </summary>
	public class UpdateProfileRequestModel
	{
		/// <summary>
		/// User first name
		/// </summary>
		/// <value>
		/// The full name.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		public string FirstName { get; set; }

	    /// <summary>
	    /// User last name
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    [Required(AllowEmptyStrings = false)]
	    public string LastName { get; set; }

        /// <summary>
        /// Change email address for login
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
       // [Required(AllowEmptyStrings = false)]
		//[EmailAddress]
		//public string Email { get; set; }

		/// <summary>
		/// Profile Image
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		public string Image { get; set; }
	}

    public class UpdateProfileRequestProfile : Profile
    {
        public UpdateProfileRequestProfile()
        {
            CreateMap<UpdateProfileRequestModel, Core.Entities.User>()
                .ForMember(m => m.Email, opt => opt.Ignore())
                .ForMember(m => m.FirstName, opt => opt.Condition(a => !string.IsNullOrWhiteSpace(a.FirstName)))
                .ForMember(m => m.LastName, opt => opt.Condition(a => !string.IsNullOrWhiteSpace(a.LastName)))
                .ForMember(m => m.Image, opt => opt.Condition(a => !string.IsNullOrWhiteSpace(a.Image)));
        }
    }
}
