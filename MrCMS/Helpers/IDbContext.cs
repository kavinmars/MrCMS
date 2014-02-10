using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Helpers
{
    public interface IDbContext : IDisposable
    {
        T Get<T>(int id) where T : SystemEntity;
        SystemEntity Get(Type type, int id);
        T Add<T>(T entity) where T : SystemEntity;
        T Update<T>(T entity) where T : SystemEntity;
        void Delete<T>(T entity) where T : SystemEntity;
        T AddOrUpdate<T>(T entity) where T : SystemEntity;
        IQueryable<TEntity> Set<TEntity>() where TEntity : SystemEntity;
        IQueryable<SystemEntity> Set(Type entityType);

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Database Database { get; }
    }
}