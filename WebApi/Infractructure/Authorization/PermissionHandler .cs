using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Exceptions.Account;
using Microsoft.AspNetCore.Authorization;
using Resources;
using Services.IdentityServices.Interfaces;

namespace WebApi.Infractructure.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAppUserManager _appUserManager;

        public PermissionHandler(IAppUserManager appUserManager)
        {
            _appUserManager = appUserManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return;
            }

            var userId = Convert.ToInt64(context.User.Claims.FirstOrDefault(x => x.Type == "sid")?.Value);

            bool hasPermission = await _appUserManager.CheckPermissionForUser(userId, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new PermissionDeniedExeption(Account.PermissionDenied);
            }
        }
    }
}
