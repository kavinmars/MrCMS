using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.DataAccess
{
    public class SetUpdatedOn : IPreCommitListener
    {
        public void OnPreCommit(DbChangeTracker tracker)
        {
            var createdOn = CurrentRequestData.Now;
            foreach (
                var entity in
                    tracker.Entries()
                        .Where(entry => (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                        .Select(entry => entry.Entity)
                        .OfType<SystemEntity>())
            {
                entity.UpdatedOn = createdOn;
            }
        }
    }
}