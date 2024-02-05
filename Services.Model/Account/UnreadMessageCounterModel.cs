using System.ComponentModel.DataAnnotations;

namespace Services.Model.Account
{
    public class UnreadMessageCounterModel
    {
        [Required]
        public int Counter { get; set; }
    }
}
