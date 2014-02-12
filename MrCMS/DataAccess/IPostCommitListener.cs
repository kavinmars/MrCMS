using System.Data.Entity.Infrastructure;

namespace MrCMS.DataAccess
{
    public interface IPostCommitListener
    {
        void OnPostCommit(DbChangeTracker tracker);
    }
}