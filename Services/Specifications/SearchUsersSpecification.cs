using System;
using System.Linq.Expressions;
using Core;
using Core.Entities;

namespace Services.Specifications
{
    public class SearchUsersSpecification : ISpecification<User>
    {
        private readonly string _searchQuery;
        private readonly long _currentUserId;

        public SearchUsersSpecification(string searchQuery, long currentUserId)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                _searchQuery = searchQuery.ToLowerInvariant().TrimEnd().TrimStart();
            }
            _currentUserId = currentUserId;
        }

        public Expression<Func<User, bool>> IsSatisfiedBy()
        { 
            ParameterExpression param = Expression.Parameter(typeof(User), "user");
            Expression<Func<User, bool>> result = user => !user.IsDelete && !user.IsAdmin && user.Id != _currentUserId;
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                Expression<Func<User, bool>> searchExpr = user => user.FirstName.Contains(_searchQuery)
                              || user.LastName.Contains(_searchQuery);

                var body = Expression.AndAlso(Expression.Invoke(result, param), Expression.Invoke(searchExpr, param));
                result = Expression.Lambda<Func<User, bool>>(body, param);
            }
            return result;
        }
    }
}
