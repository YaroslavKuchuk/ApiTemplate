using System.ComponentModel.DataAnnotations;

namespace Services.Model.User
{
    public class IsEmailUniqueModel
    {
        /// <summary>
		/// Email to be verified.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter 'Email'")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
