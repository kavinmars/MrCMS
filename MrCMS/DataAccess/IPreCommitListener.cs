using System.Data.Entity.Infrastructure;

namespace MrCMS.DataAccess
{
    public interface IPreCommitListener
    {
        void OnPreCommit(DbChangeTracker tracker);
    }
}