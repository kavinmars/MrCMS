using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.DataAccess
{
    public interface IPreCommitListener
    {
        void OnPreCommit(DbChangeTracker tracker);
    }

    public class SoftDelete : IPreCommitListener
    {
        public void OnPreCommit(DbChangeTracker tracker)
        {
            foreach (var entityEntry in tracker.Entries()
                .Where(entry => entry.State == EntityState.Deleted))
            {
                entityEntry.State = EntityState.Modified;
                (entityEntry.Entity as SystemEntity).IsDeleted = true;
            }
        }
    }
}