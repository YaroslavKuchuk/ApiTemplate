using Core.Entities;
using System.Collections.Generic;

namespace Core.IdentityEntities
{
    public class AppIdentityPermission : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        

        public virtual ICollection<AppRolePermission> Roles { get; set; }
    }
}
