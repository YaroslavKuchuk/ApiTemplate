using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class ActivationCode : BaseEntity
    {
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(255)]
        public string EntityActivation { get; set; } 
        public bool IsActivated { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
