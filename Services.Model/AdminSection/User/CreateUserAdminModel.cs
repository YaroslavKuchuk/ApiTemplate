using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Services.Model.AdminSection.User
{
    public class CreateUserAdminModel
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
        /// Sets the country id.
        /// </summary>
        /// <value>
        /// The country id.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Country'")]
        public long CountryId { get; set; }

        /// <summary>
        /// Sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Sets the city.
        /// </summary>
        /// <value>
        /// The city.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'City'")]
        public string City { get; set; }

        /// <summary>
		/// Sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
        public string State { get; set; }

        /// <summary>
        /// Sets the zip.
        /// </summary>
        /// <value>
        /// The zip.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Zip'")]
        public string Zip { get; set; }

        /// <summary>
        /// Sets the industry.
        /// </summary>
        /// <value>
        /// The industry.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Industry'")]
        public string Industry { get; set; }

        /// <summary>
        /// Sets the birthday.
        /// </summary>
        /// <value>
        /// The birthday.
        /// </value>
        [Required(ErrorMessage = "Please enter 'Birthday'")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Sets the company name.
        /// </summary>
        /// <value>
        /// The company name.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Company name'")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Position'")]
        public string Position { get; set; }

        /// <summary>
        /// Sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Sets the avatar image url.
        /// </summary>
        /// <value>
        /// The avatar image url.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Password'"), StringLength(30, MinimumLength = 6, ErrorMessage = "'Password' is too short. Passwords must be between 6 and 30 characters")]
        public string Password { get; set; }

        /// <summary>
        /// Sets the confirm password.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [Compare("Password", ErrorMessage = "'Confirm Password' and 'Password' fields do not match")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Confirm password'"), StringLength(30, MinimumLength = 6, ErrorMessage = "'Confirm Password' is too short. Passwords must be between 6 and 30 characters")]
        public string ConfirmPassword { get; set; }
    }

    public class CreateUserAdminProfile : Profile
    {
        public CreateUserAdminProfile()
        {
          CreateMap<CreateUserAdminModel, Core.Entities.User>()
                .ForMember(m => m.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(m => m.UpdateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.CreateDate, opt => opt.MapFrom(src =>DateTime.UtcNow))
                .ForMember(m => m.IsActive, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.IsAdmin, opt => opt.MapFrom(src =>false))
                .ForMember(m => m.EmailConfirmed, opt => opt.MapFrom(src =>true))
                .ForMember(m => m.EnablePush, opt => opt.MapFrom(src =>true));
        }
    }
}
