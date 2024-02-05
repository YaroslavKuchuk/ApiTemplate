using System.ComponentModel.DataAnnotations;

namespace Services.Model.AdminSection.User
{
    public class ChangeAdminPasswordModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public long UserId { get; set; }

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
