using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Services
{
    public interface ICurrentSiteLocator
    {
        Site GetCurrentSite();
    }

    public class CurrentSiteLocator : ICurrentSiteLocator
    {
        private readonly ISession _session;
        private readonly IOwinContext _owinContext;
        private Site _currentSite;
        public CurrentSiteLocator(ISession session, IOwinContext owinContext)
        {
            _session = session;
            _owinContext = owinContext;
        }

        public Site GetCurrentSite()
        {
            return _currentSite ?? (_currentSite = GetSiteFromSettingForDebugging() ?? GetSiteFromRequest());
        }

        private Site GetSiteFromSettingForDebugging()
        {
            var appSetting = ConfigurationManager.AppSettings["debugSiteId"];

            int id;
            return int.TryParse(appSetting, out id) ? _session.Get<Site>(id) : null;
        }

        private Site GetSiteFromRequest()
        {
            var authority = _owinContext.Request.Uri.Authority;

            var allSites = _session.QueryOver<Site>().Fetch(s => s.RedirectedDomains).Eager.Cacheable().List();
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