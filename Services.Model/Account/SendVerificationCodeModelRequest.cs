using System.ComponentModel.DataAnnotations;


namespace Services.Model.Account
{
    public class SendVerificationCodeModelRequest
    {
        // [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        public string Entity { get; set; }

        public bool IsEmail { get; set; }
    }
}
