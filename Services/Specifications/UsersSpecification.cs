using System;
using System.Linq.Expressions;
using Core;
using Core.Entities;

namespace Services.Specifications
{
    public class UsersSpecification : ISpecification<User>
    {
        private readonly string _searchQuery;

        public UsersSpecification(string searchQuery)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                _searchQuery = searchQuery.ToLowerInvariant().TrimEnd().TrimStart();
            }
        }
        public Expression<Func<User, bool>> IsSatisfiedBy()
        {
            ParameterExpression param = Expression.Parameter(typeof(User), "user");
            Expression<Func<User, bool>> result = user => !user.IsDelete && !user.IsAdmin;
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                Expression<Func<User, bool>> searchExpr = user => user.FirstName.Contains(_searchQuery)
                              || user.LastName.Contains(_searchQuery)
                              || user.Email.Contains(_searchQuery);
                var body = Expression.AndAlso(Expression.Invoke(result, param), Expression.Invoke(searchExpr, param));
                result = Expression.Lambda<Func<User, bool>>(body, param);
            }
            return result;
        }
    }
}
