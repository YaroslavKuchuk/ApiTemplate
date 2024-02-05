using Core.Data;
using Core.Data.Repositories;
using Core.IdentityEntities;
using Services.IdentityServices.Enum;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AppRolePermission> _permissionRepository;


        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _permissionRepository = _unitOfWork.Repository<AppRolePermission>();
        }

        public async Task<bool> CheckPermissionInRole(List<string> roles, Permission permission)
        {
           return await _permissionRepository.AnyAsync(rp => roles.Contains(rp.Role.Name) && rp.PermissionId == (int)permission);
        }
    }
}
