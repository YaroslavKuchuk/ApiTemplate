using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
    public class InstagramLoginRequestModel : BaseAuthModel
    {
        [Required]
        public string AuthorizationCode { get; set; }
    }
}
