using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Core.IdentityEntities
{
    public class AppRole : IdentityRole<long> 
    {
        public virtual ICollection<AppRolePermission> Permissions { get; set; }
        public AppRole()
        {
		}

		public AppRole(string name)
        {
			Name = name;
        }
    }
}
