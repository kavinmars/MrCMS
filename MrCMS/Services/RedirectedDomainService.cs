using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class RedirectedDomainService : IRedirectedDomainService
    {
        private readonly IDbContext _dbContext;

        public RedirectedDomainService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Save(RedirectedDomain domain)
        {
            if (domain.Site != null)
                domain.Site.RedirectedDomains.Add(domain);
            _dbContext.Transact(dbContext => dbContext.Add(domain));
        }

        public void Delete(RedirectedDomain domain)
        {
            if (domain.Site != null)
                domain.Site.RedirectedDomains.Remove(domain);
            _dbContext.Transact(dbContext => dbContext.Delete(domain));
        }
    }
}