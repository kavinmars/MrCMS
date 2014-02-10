using System.Data.Entity.Infrastructure;

namespace MrCMS.Helpers
{
    public class DbContextFactory : IDbContextFactory<MrCMSDbContext>, IDbContextFactory
    {
        private IDbConfiguration _dbConfiguration = new DbConfiguration();

        public void SetDbConfiguration(IDbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;
        }

        public IDbContext GetContext()
        {
            return
                new StandardDbContext(Create());
        }

        public MrCMSDbContext Create()
        {
            return new MrCMSDbContext(_dbConfiguration);
        }
    }
}