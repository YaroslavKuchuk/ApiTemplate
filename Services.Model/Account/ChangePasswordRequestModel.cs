using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
	/// <summary>
	/// Change password model
	/// </summary>
	public class ChangePasswordRequestModel
	{
		/// <summary>
		/// Compare value with existing password
		/// </summary>
		/// <value>
		/// The current password.
		/// </value>
		public string CurrentPassword { get; set; }

		/// <summary>
		/// Gets or sets the new password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; }

		/// <summary>
		/// Compare if new password correct
		/// </summary>
		/// <value>
		/// The confirm password.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		[StringLength(100, MinimumLength = 6)]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }
	}
}
