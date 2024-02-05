using Core.Entities;

namespace Core.IdentityEntities
{
    public class AppRolePermission : BaseEntity
    {
        public long RoleId { get; set; }
        public /*virtual*/ AppRole Role { get; set; }

        public long PermissionId { get; set; }
        public /*virtual*/ AppIdentityPermission Permission { get; set; }
    }
}
