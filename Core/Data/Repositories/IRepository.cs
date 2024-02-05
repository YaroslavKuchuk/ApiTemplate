using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Data.Repositories.Enum;

namespace Core.Data.Repositories
{
	public interface IRepository<TEntity> : IDisposable where TEntity : IBaseEntity
	{
		IQueryable<TEntity> GetAllQueryable();
        /// <summary>
        /// Use this method or GetAllQueryable to load multiple inner entities via Include/ThenInclude. Lazy loading is disabled for EntityFramework Core 2.0.1
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
	    IQueryable<TEntity> GetFilteredQueryable(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetFilteredQueryableIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        List<TEntity> GetAll();
	    List<TEntity> GetAllAsNoTracking();
        List<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
	    List<TEntity> GetFilteredIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetSingle(long id);
		TEntity GetSingleIncluding(long id, params Expression<Func<TEntity, object>>[] includeProperties);
		List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
	    IQueryable<TEntity> QueryBySpecificationList(ISpecification<TEntity>[] includeProperties);
	    IQueryable<TEntity> QueryBySpecificationListIncluding(ISpecification<TEntity>[] specification, params Expression<Func<TEntity, object>>[] includeProperties);

        void Update(TEntity entity);
		void Delete(TEntity entity);
        void DeleteEntities(IEnumerable<TEntity> entities);

        Task Insert(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
	    Task<List<TEntity>> GetAllAsNoTrackingAsync();
        Task<List<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties);
	    Task<List<TEntity>> GetFilteredIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> GetAllAsync(int count, int offset, Expression<Func<TEntity, DateTime>> keySelector,
				Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(long id);
		Task<TEntity> GetSingleIncludingAsync(long id, params Expression<Func<TEntity, object>>[] includeProperties);
		Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate);
    }
}