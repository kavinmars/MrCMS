using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;

namespace MrCMS.DataAccess
{
    public class SetSite : IPreCommitListener
    {
        private readonly Site _site;

        public SetSite(Site site)
        {
            _site = site;
        }

        public void OnPreCommit(DbChangeTracker tracker)
        {
            foreach (
                var entity in
                    tracker.Entries()
                        .Where(entry => (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                        .Select(entry => entry.Entity)
                        .OfType<SiteEntity>().Where(entity => entity.Site == null))
            {
                entity.Site = _site;
            }
        }
    }
}