using System.Collections.Generic;
using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Services
{
    public class SiteService : ISiteService
    {
        private readonly IDbContext _dbContext;
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IIndexService _indexService;

        public SiteService(IDbContext dbContext, ICloneSiteService cloneSiteService, IIndexService indexService)
        {
            _dbContext = dbContext;
            _cloneSiteService = cloneSiteService;
            _indexService = indexService;
        }

        public List<Site> GetAllSites()
        {
            return _dbContext.Query<Site>().OrderBy(site => site.Name).ToList();
        }

        public Site GetSite(int id)
        {
            return _dbContext.Get<Site>(id);
        }

        public void AddSite(Site site, SiteCopyOptions options)
        {
            _dbContext.Transact(session => session.Add(site));
            _indexService.InitializeAllIndices(site);

            if (options.SiteId.HasValue)
            {
                _cloneSiteService.CloneData(site, options);
            }
        }

        public void SaveSite(Site site)
        {
            _dbContext.Transact(session => session.Update(site));
        }

        public void DeleteSite(Site site)
        {
            _dbContext.Transact(session =>
                                  {
                                      site.OnDeleting(session);
                                      session.Delete(site);
                                  });
        }
    }
}