using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ICloneSiteService
    {
        void CloneData(Site site, SiteCopyOptions options);
    }

    public class CloneSiteService : ICloneSiteService
    {
        private readonly ICloneSitePartsService _cloneSitePartsService;
        private readonly IDbContext _dbContext;

        public CloneSiteService(ICloneSitePartsService cloneSitePartsService, IDbContext dbContext)
        {
            _cloneSitePartsService = cloneSitePartsService;
            _dbContext = dbContext;
        }

        public void CloneData(Site site, SiteCopyOptions options)
        {
            if (!options.SiteId.HasValue)
                return;

            var @from = _dbContext.Get<Site>(options.SiteId.Value);
            if (@from == null)
                return;
            _cloneSitePartsService.CopySettings(@from, site);
            if (options.CopyLayouts)
                _cloneSitePartsService.CopyLayouts(@from, site);
            if (options.CopyMediaCategories)
                _cloneSitePartsService.CopyMediaCategories(@from, site);
            if (options.CopyHome)
                _cloneSitePartsService.CopyHome(@from, site);
            if (options.Copy404)
                _cloneSitePartsService.Copy404(@from, site);
            if (options.Copy403)
                _cloneSitePartsService.Copy403(@from, site);
            if (options.Copy500)
                _cloneSitePartsService.Copy500(@from, site);
            if (options.CopyLogin)
                _cloneSitePartsService.CopyLogin(@from, site);
        }
    }
}