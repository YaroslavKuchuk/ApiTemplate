using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data.Repositories;

namespace Core.Data
{
	public interface IUnitOfWork : IDisposable
	{
		int SaveChanges();
		void Dispose(bool disposing);
		IRepository<TEntity> Repository<TEntity>() where TEntity : class, IBaseEntity;
        void BeginTransaction();
		int Commit();
		void Rollback();
		Task<int> SaveChangesAsync();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
		Task<int> CommitAsync();
	    Task DeleteUser(long userId);
	    Task ExecuteSqlCommand(string command);
	    string GetDbName();
	    string GetTableName<TEntity>() where TEntity : class, IBaseEntity;
    }
}