using System;
using System.Linq.Expressions;
using Core;
using Core.Entities;

namespace Services.Specifications
{
    public class AdminUserSpecification : ISpecification<User>
    {
        public Expression<Func<User, bool>> IsSatisfiedBy()
        {
            //var roleManager = new AppRoleManager(new AppRoleStore(new AppDbContext()));
            //var userRole = roleManager.Roles.Single(role => role.Name == Role.User.ToString());
           // return user => user.Roles.All(role => role.RoleId != userRole.Id);
            return user => !user.IsDelete && user.IsAdmin;
        }
    }
}
