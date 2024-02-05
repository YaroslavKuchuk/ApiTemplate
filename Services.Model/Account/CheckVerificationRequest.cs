using System.ComponentModel.DataAnnotations;


namespace Services.Model.Account
{
    public class CheckVerificationRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }

       // [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        public string Entity { get; set; }
    }
}
