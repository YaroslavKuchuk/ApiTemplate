using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Data.Repositories;
using Data.Repositories;

namespace Data
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly IContext _context;
		private bool _disposed;
		private Hashtable _repositories;

		public UnitOfWork(IContext context)
		{
			_context = context;
		}

		public int SaveChanges()
		{
			return _context.SaveChanges();
		}
		
		public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IBaseEntity
		{
			if (_repositories == null)
			{
				_repositories = new Hashtable();
			}
			var type = typeof(TEntity).Name;
			if (_repositories.ContainsKey(type))
			{
				return (IRepository<TEntity>)_repositories[type];
			}
			var repositoryType = typeof(EntityRepository<>);
			_repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context));

			return (IRepository<TEntity>)_repositories[type];
		}

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}

		public void BeginTransaction()
		{
			_context.BeginTransaction();
		}

		public int Commit()
		{
			return _context.Commit();
		}

		public void Rollback()
		{
			_context.Rollback();
		}

		public Task<int> CommitAsync()
		{
			return _context.CommitAsync();
		}

	    public async Task DeleteUser(long userId)
	    {
	        await _context.DeleteUser(userId);
	    }

	    public async Task ExecuteSqlCommand(string command)
	    {
            await _context.ExecuteSqlCommand(command);
        }

	    public string GetDbName()
	    {
	        return _context.GetDbName();
	    }

	    public string GetTableName<TEntity>() where TEntity : class, IBaseEntity
	    {
	        return _context.GetTableName<TEntity>();
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

				foreach (IDisposable repository in _repositories.Values)
				{
					repository.Dispose();
				}
			}

			_disposed = true;
		}
	}
}
