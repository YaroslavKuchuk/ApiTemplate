using System;
using System.Linq.Expressions;
using Core.Data;

namespace Core
{
    public interface ISpecification<T> where T : IBaseEntity
    {
        Expression<Func<T, bool>> IsSatisfiedBy();
    }
}
