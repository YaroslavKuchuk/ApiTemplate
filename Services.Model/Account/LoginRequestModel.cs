using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
	/// <summary>
	/// Login Model
	/// </summary>
	public class LoginRequestModel : BaseAuthModel
	{
		/// <summary>
		/// Email for login
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Password for login
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; }
	}
}
