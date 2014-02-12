using System.Data.Entity.Infrastructure;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.DataAccess
{
    public class DbContextFactory : IDbContextFactory<MrCMSDbContext>, IDbContextFactory
    {
        public IDbContext GetContext()
        {
            return
                new StandardDbContext(Create());
        }

        public MrCMSDbContext Create()
        {
            return new MrCMSDbContext(MrCMSApplication.GetAll<IPreCommitListener>());
        }
    }
}