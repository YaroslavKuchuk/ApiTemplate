using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
	/// <summary>
	/// Reset password
	/// </summary>
	public class ResetPasswordModel
	{
		/// <summary>
		/// User email for reset
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[Required(AllowEmptyStrings = false)]
		[EmailAddress]
		public string Email { get; set; }
	}

	public class SendPasswordCallback
	{
		public Core.Entities.User User { get; set; }
		public string Password { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
