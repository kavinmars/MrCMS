using System;
using System.Configuration;
using System.Linq;
using System.Web;
using MrCMS.DataAccess;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using System.Data.Entity;

namespace MrCMS.Services
{
    public interface ICurrentSiteLocator
    {
        Site GetCurrentSite();
    }

    public class CurrentSiteLocator : ICurrentSiteLocator
    {
        private readonly IDbContext _dbContext;
        private readonly HttpRequestBase _requestBase;
        private Site _currentSite;
        public CurrentSiteLocator(IDbContext dbContext, HttpRequestBase requestBase)
        {
            _dbContext = dbContext;
            _requestBase = requestBase;
        }

        public Site GetCurrentSite()
        {
            return _currentSite ?? (_currentSite = GetSiteFromSettingForDebugging() ?? GetSiteFromRequest());
        }

        private Site GetSiteFromSettingForDebugging()
        {
            var appSetting = ConfigurationManager.AppSettings["debugSiteId"];

            int id;
            return int.TryParse(appSetting, out id) ? _dbContext.Get<Site>(id) : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = _requestBase.Url.Authority;

            var allSites = _dbContext.Query<Site>().Include(s => s.RedirectedDomains).ToList();
            var redirectedDomains = allSites.SelectMany(s => s.RedirectedDomains).ToList();
            var site = allSites.FirstOrDefault(s => s.BaseUrl != null && s.BaseUrl.Equals(authority, StringComparison.OrdinalIgnoreCase));
            if (site == null)
            {
                var redirectedDomain =
                    redirectedDomains.FirstOrDefault(
                        s => s.Url != null && s.Url.Equals(authority, StringComparison.OrdinalIgnoreCase));
                if (redirectedDomain != null)
                    site = redirectedDomain.Site;
            }

            return site ?? allSites.First();
        }
    }
}