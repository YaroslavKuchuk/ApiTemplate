using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Services.Model.Account
{
	/// <summary>
	/// Registration model
	/// </summary>
	public class RegistrationRequestModel : BaseAuthModel
	{
        /// <summary>
        /// Sets the new email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Email'")]
        [EmailAddress]
		public string Email { get; set; }

        /// <summary>
        /// Sets the first name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'First name'")]
        public string FirstName { get; set; }

	    /// <summary>
	    /// Sets the last name.
	    /// </summary>
	    /// <value>
	    /// The full name.
	    /// </value>
	    [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Last name'")]
	    public string LastName { get; set; }


	    /// <summary>
	    /// Sets the avatar image in base64 format.
	    /// </summary>
	    /// <value>
	    /// The avatar image url.
	    /// </value>
	    public string AvatarFile { get; set; }

	    /// <summary>
	    /// Sets the avatar image's extension.
	    /// </summary>
	    /// <value>
	    /// The avatar image url.
	    /// </value>
	    public string AvatarFileType { get; set; }

	    /// <summary>
	    /// Sets the password.
	    /// </summary>
	    /// <value>
	    /// The password.
	    /// </value>
	    [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Password'"), StringLength(30, MinimumLength = 6, ErrorMessage = "You gave a too short password. It needs contain at least 6 characters.")]
	    public string Password { get; set; }

	    /// <summary>
	    /// Sets the confirm password.
	    /// </summary>
	    /// <value>
	    /// The address.
	    /// </value>
	    [Compare("Password", ErrorMessage = "'Confirm Password' and 'Password' fields do not match")]
	    [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Confirm password'"), StringLength(30, MinimumLength = 6, ErrorMessage = "You gave a too short password. It needs contain at least 6 characters.")]
	    public string ConfirmPassword { get; set; }
	}

    public class RegistrationRequestProfile : Profile
    {
        public RegistrationRequestProfile()
        {
           CreateMap<RegistrationRequestModel, Core.Entities.User>()
                .ForMember(m => m.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(x => x.FirstName))
               .ForMember(m => m.LastName, opt => opt.MapFrom(x => x.LastName))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>false))
               .ForMember(m => m.EnablePush, opt => opt.MapFrom(src => true));
        }
    }
}
