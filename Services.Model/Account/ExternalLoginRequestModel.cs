using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
    public class ExternalLoginRequestModel: BaseAuthModel
	{
		[Required]
		[Display(Name = "External access token")]
		public string ExternalAccessToken { get; set; }
	}
}
