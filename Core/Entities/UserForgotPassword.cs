using Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class UserForgotPassword : BaseEntity
    {        
        public long UserId { get; set; }
        [MaxLength(36)]
        public string Guid { get; set; }
        public UserForgotPasswordStatus Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }

        public  /*virtual*/ User User { get; set; }
    }
}
