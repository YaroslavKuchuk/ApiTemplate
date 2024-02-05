using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.Entities;
using Core.Entities.Notification;
using Core.IdentityEntities;
using Data.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Data
{
    public class AppDbContext : IdentityDbContext<User, AppRole, long>, IContext
    {
		private IDbContextTransaction _transaction;
		private static readonly object _lock = new object();
		private static bool _databaseInitialized;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            if (_databaseInitialized)
			{
				return;
			}
			lock (_lock)
			{
				if (!_databaseInitialized)
				{
					_databaseInitialized = true;
				}
			}
		}

        public virtual DbSet<Setting> Settings { get; set; }
		public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<UserToken> AppUserTokens { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<ActivationCode> ActivationCodes { get; set; }
        public virtual DbSet<UserForgotPassword> UserForgotPasswords { get; set; }
        public virtual DbSet<AppIdentityPermission> AppIdentityPermissions { get; set; }
        public virtual DbSet<AppRolePermission> AppRolePermissions { get; set; }       


        /* Notifications */
        public virtual DbSet<QueueMessage> Messages { get; set; }
        public virtual DbSet<MessageHistory> MessagesHistory { get; set; }
        public virtual DbSet<AmazonTopic> AmazonTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
		    base.OnModelCreating(builder);

            EfConfig.Configure(builder);
		    
		    builder.ApplyConfiguration(new UserMapping());
		    builder.ApplyConfiguration(new AppUserLoginMapping());
		    builder.ApplyConfiguration(new AppRoleMapping());
            builder.ApplyConfiguration(new LogMapping());
		    builder.ApplyConfiguration(new QueueMessageMapping());
		    builder.ApplyConfiguration(new SettingMapping());
		    builder.ApplyConfiguration(new EFMigrationsHistoryMapping());
        }

        public void SetAsAdded<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
		{
			UpdateEntityState(entity, EntityState.Added);
		}

		public void SetAsModified<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
		{
			UpdateEntityState(entity, EntityState.Modified);
		}

		public void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
		{
			UpdateEntityState(entity, EntityState.Deleted);
		}

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            var connection = Database.GetDbConnection();
            if (connection.State == ConnectionState.Open)
            {
                return;
            }
            Database.OpenConnection();
            _transaction = Database.BeginTransaction();
        }

		public int Commit()
		{
			var saveChanges = SaveChanges();
			_transaction.Commit();
			return saveChanges;
		}

		public void Rollback()
		{
			_transaction.Rollback();
		}

		public Task<int> CommitAsync()
		{
			var saveChangesAsync = SaveChangesAsync();
			_transaction.Commit();

			return saveChangesAsync;
		}

		private void UpdateEntityState<TEntity>(TEntity entity, EntityState entityState) where TEntity : class, IBaseEntity
		{
			var dbEntityEntry = GetDbEntityEntrySafely(entity);
			dbEntityEntry.State = entityState;
		}

		private EntityEntry<TEntity> GetDbEntityEntrySafely<TEntity>(TEntity entity) where TEntity : class, IBaseEntity
		{
			var dbEntityEntry = Entry(entity);
			if (dbEntityEntry.State == EntityState.Detached)
			{
				Set<TEntity>().Attach(entity);
			}

			return dbEntityEntry;
		}

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Database != null && Database.GetDbConnection().State == ConnectionState.Open)
                {
                    Database.CloseConnection();
                }
                _transaction?.Dispose();
            }
            base.Dispose();
        }

        public async Task DeleteUser(long userId)
        {
            try
            {
                var userIdParameter = new SqlParameter("@UserId", userId);
                var res = await Database.ExecuteSqlCommandAsync("EXEC DeleteUser @UserId", userIdParameter); 
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task ExecuteSqlCommand(string command)
        {
            var res = await Database.ExecuteSqlCommandAsync(command);
        }

        public string GetDbName()
        {
            return Database.GetDbConnection().Database;
        }

        public string GetTableName<TEntity>() where TEntity : class, IBaseEntity
        {
            // DbContext knows everything about the model.
            var entityTypes = Model.GetEntityTypes();
            // ... and get one by entity type information of "TEntity" DbSet property.
            var entityType = entityTypes.First(t => t.ClrType == typeof(TEntity));
            // The entity type information has the actual table name as an annotation!
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            return tableNameAnnotation.Value.ToString();
        }
    }
}
