using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Infractructure.Authorization
{
    public class IsUserValidHandler : AuthorizationHandler<IsUserValidRequirement>
    {
        private readonly UserManager<User> _userManager;

        public IsUserValidHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsUserValidRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return;
            }
            var userId = Convert.ToInt64(context.User.Claims.FirstOrDefault(x => x.Type == "sid")?.Value);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.IsActive)
            {
                context.Succeed(requirement);
            }
            else if(user == null)
            {
                throw new ApiException("Please re-login", HttpStatusCode.Unauthorized);
            }
            else if(!user.IsActive)
            {
                throw new ApiException("User is blocked", HttpStatusCode.Unauthorized);
            }
        }
    }
}
