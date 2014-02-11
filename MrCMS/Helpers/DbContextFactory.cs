using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MrCMS.Helpers
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
            return new MrCMSDbContext();
        }
    }
}