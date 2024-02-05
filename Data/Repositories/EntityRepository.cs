using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core;
using Core.Data;
using Core.Data.Repositories;
using Core.Data.Repositories.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbEntitySet;
        private bool _disposed;

        public EntityRepository(DbContext context)
        {
            _context = context;
            _dbEntitySet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllQueryable()
        {
            return _dbEntitySet.AsQueryable();
        }

        public IQueryable<TEntity> GetFilteredQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate).AsQueryable();
        }

        public IQueryable<TEntity> GetFilteredQueryableIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return IncludeConditionalProperties(predicate, includeProperties);
        }

        public List<TEntity> GetAll()
        {
            return _dbEntitySet.ToList();
        }

        public List<TEntity> GetAllAsNoTracking()
        {
            return _dbEntitySet.AsNoTracking().ToList();
        }

        public List<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.ToList();
        }

        public List<TEntity> GetFilteredIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeConditionalProperties(predicate, includeProperties);
            return entities.ToList();
        }

        public TEntity GetSingle(long id)
        {
            return _dbEntitySet.Find(id);
        }

        public TEntity GetSingleIncluding(long id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);

            return entities.FirstOrDefault(x => x.Id == id);
        }

        public List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate).ToList();
        }

        public IQueryable<TEntity> QueryBySpecificationList(ISpecification<TEntity>[] includeProperties)
        {
            IQueryable<TEntity> result = _dbEntitySet;
            foreach (var property in includeProperties)
                result = result.Where(property.IsSatisfiedBy());
            return result;
        }

        public IQueryable<TEntity> QueryBySpecificationListIncluding(ISpecification<TEntity>[] specification, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> result = IncludeProperties(includeProperties);
            foreach (var property in specification)
                result = result.Where(property.IsSatisfiedBy());
            return result;
        }

        public async Task Insert(TEntity entity)
        {
            await _context.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity is IIsDeleteBaseEntity)
            {
                ((IIsDeleteBaseEntity)entity).IsDelete = true;
                _context.Update(entity);
            }
            else
            {
                _context.Remove(entity);
            }
        }

        public void DeleteEntities(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                return;
            if (entities.FirstOrDefault() is IIsDeleteBaseEntity)
            {
                foreach (var entity in entities)
                {
                    ((IIsDeleteBaseEntity)entity).IsDelete = true;
                }
                _context.Update(entities);
            }
            else
            {
                _context.RemoveRange(entities);
            }
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return _dbEntitySet.ToListAsync();
        }

        public Task<List<TEntity>> GetAllAsNoTrackingAsync()
        {
            return _dbEntitySet.AsNoTracking().ToListAsync();
        }

        public Task<List<TEntity>> GetFilteredIncludingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeConditionalProperties(predicate, includeProperties);
            return entities.ToListAsync();
        }

        public async Task<List<TEntity>> GetAllAsync(int count, int offset, Expression<Func<TEntity, DateTime>> keySelector,
            Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = FilterQuery(keySelector, predicate, orderBy, includeProperties);

            entities = entities.Skip(offset).Take(count);

            return await entities.ToListAsync();
        }

        public Task<List<TEntity>> GetAllIncludingAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.ToListAsync();
        }

        public Task<TEntity> GetSingleAsync(long id)
        {
            return _dbEntitySet.FindAsync(id);
        }

        public Task<TEntity> GetSingleIncludingAsync(long id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            return entities.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Where(predicate).ToListAsync();
        }

        private IQueryable<TEntity> IncludeProperties(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return includeProperties.Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(_dbEntitySet, (current, includeProperty) => current.Include(includeProperty));
        }

        private IQueryable<TEntity> IncludeConditionalProperties(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return includeProperties.Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(_dbEntitySet.Where(predicate), (current, includeProperty) => current.Include(includeProperty));
        }

        private IQueryable<TEntity> FilterQuery(Expression<Func<TEntity, DateTime>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderBy orderBy, Expression<Func<TEntity, object>>[] includeProperties)
        {
            var entities = IncludeProperties(includeProperties);
            entities = (predicate != null) ? entities.Where(predicate) : entities;
            entities = (orderBy == OrderBy.Ascending)
                ? entities.OrderBy(keySelector)
                : entities.OrderByDescending(keySelector);

            return entities;
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.AnyAsync(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbEntitySet.Any(predicate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
