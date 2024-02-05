using Microsoft.AspNetCore.Authorization;
using Services.IdentityServices.Enum;

namespace WebApi.Infractructure.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; }
    }
}
