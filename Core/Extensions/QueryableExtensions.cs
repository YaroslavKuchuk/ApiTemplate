using System;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;
using Core.Enums.Orders;
using Microsoft.EntityFrameworkCore;

namespace Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Fetch<T>(this IQueryable<T> source, params Expression<Func<T, object>>[] pathes) where T: class 
        {
            return pathes.Aggregate(source, (current, path) => current.Include(path));
        }

        public static IOrderedQueryable<User> SetOrder(this IQueryable<User> source, UserOrderBy? orderBy)
        {
            switch (orderBy)
            {
                case UserOrderBy.Name:
                    return source.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
                case null:
                case UserOrderBy.NameDesc:
                    return source.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName);
                case UserOrderBy.Email:
                    return source.OrderBy(x => x.Email);
                case UserOrderBy.EmailDesc:
                    return source.OrderByDescending(x => x.Email);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null);
            }
        }
        
    }
}