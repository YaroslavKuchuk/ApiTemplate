using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;

namespace Data
{
	public interface IContext : IDisposable
	{
		void SetAsAdded<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;
		void SetAsModified<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;
		void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class, IBaseEntity;
		int SaveChanges();
		Task<int> SaveChangesAsync();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
		void BeginTransaction();
		int Commit();
		void Rollback();
		Task<int> CommitAsync();
	    Task DeleteUser(long userId);
	    Task ExecuteSqlCommand(string command);
	    string GetDbName();
	    string GetTableName<TEntity>() where TEntity : class, IBaseEntity;
	}
}