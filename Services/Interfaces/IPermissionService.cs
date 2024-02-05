using Services.IdentityServices.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> CheckPermissionInRole(List<string> roles, Permission permission);
    }
}
