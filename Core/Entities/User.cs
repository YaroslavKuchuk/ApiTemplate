using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class User : IdentityUser<long>, IIsDeleteBaseEntity
    {
        public User()
        {
            UserTokens = new HashSet<UserToken>();
        }

        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        public string Image { get; set; }
        public bool EnablePush { get; set; }
        public long UnreadMessageCount { get; set; }
        public string FbToken { get; set; }

        public bool EnableNotifications { get; set; }

        public bool IsActive { get; set; }

        public bool IsAdmin { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdateDate { get; set; }

        public bool IsDelete { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }
    }
}
