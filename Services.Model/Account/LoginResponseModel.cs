using Newtonsoft.Json;

namespace Services.Model.Account
{
	/// <summary>
	/// Response model
	/// </summary>
	public class LoginResponseModel
	{
		/// <summary>
		/// User identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		public long UserId { get; set; }

		/// <summary>
		/// Bearer token for login
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		[JsonProperty("access_token")]
		public string Token { get; set; }

		/// <summary>
		/// User Email
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("userName")]
		public string UserName { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; }

		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		public string Image { get; set; }
	}
}
