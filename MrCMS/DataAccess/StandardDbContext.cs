using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.DataAccess
{
    public class StandardDbContext : IDbContext
    {
        private readonly MrCMSDbContext _dbContext;

        public StandardDbContext(MrCMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Database Database { get { return _dbContext.Database; } }
        public DbChangeTracker ChangeTracker { get { return _dbContext.ChangeTracker; } }
        public DbContextConfiguration Configuration { get { return _dbContext.Configuration; } }
        public IQueryable<TEntity> Query<TEntity>() where TEntity : SystemEntity
        {
            return _dbContext.Set<TEntity>();
        }

        public IQueryable<SystemEntity> Query(Type entityType)
        {
            return _dbContext.Set(entityType).OfType<SystemEntity>().AsQueryable();
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            return _dbContext.GetValidationErrors();
        }

        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return _dbContext.Entry(entity);
        }

        public DbEntityEntry Entry(object entity)
        {
            return _dbContext.Entry(entity);
        }

        public T Get<T>(int id) where T : SystemEntity
        {
            return _dbContext.Set<T>().Find(id);
        }

        public T GetInThisContext<T>(T entity) where T : SystemEntity
        {
            return Get<T>(entity.Id);
        }

        public SystemEntity Get(Type type, int id)
        {
            return _dbContext.Set(type).Find(id) as SystemEntity;
        }

        public T Add<T>(T entity) where T : SystemEntity
        {
            _dbContext.Set<T>().Add(entity);
            return entity;
        }

        public T Update<T>(T entity) where T : SystemEntity
        {
            _dbContext.Entry(entity).Property(arg => arg.UpdatedOn).IsModified = true;
            return entity;
        }

        public void Delete<T>(T entity) where T : SystemEntity
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public T AddOrUpdate<T>(T entity) where T : SystemEntity
        {
            _dbContext.Set<T>().AddOrUpdate(arg => arg.Id, entity);
            return entity;
        }
    }
}