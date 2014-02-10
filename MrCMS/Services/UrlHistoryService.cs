using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public class UrlHistoryService : IUrlHistoryService
    {
        private readonly IDbContext _dbContext;

        public UrlHistoryService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(UrlHistory urlHistory)
        {
            _dbContext.Transact(session => _dbContext.Delete(urlHistory));
        }

        public void Add(UrlHistory urlHistory)
        {
            urlHistory.Webpage.Urls.Add(urlHistory);
            _dbContext.Transact(session => session.Add(urlHistory));
        }

        public IEnumerable<UrlHistory> GetAllOtherUrls(Webpage document)
        {
            var urlHistory = _dbContext.Set<UrlHistory>().Where(x => x.Webpage.Id != document.Id).ToList();
            var urls = _dbContext.Set<Document>().Where(x => x.Id != document.Id).ToList();
            foreach (var url in urls)
            {
                if (urlHistory.All(x => x.UrlSegment != url.UrlSegment))
                    urlHistory.Add(new UrlHistory() { UrlSegment = url.UrlSegment, Webpage = document });
            }
            return urlHistory;
        }

        public UrlHistory GetByUrlSegment(string url)
        {
            return _dbContext.Set<UrlHistory>().FirstOrDefault(x => x.Site == CurrentRequestData.CurrentSite && x.UrlSegment.Contains(url));
        }

        public UrlHistory GetByUrlSegmentWithSite(string url, Site site, Webpage page)
        {
            return _dbContext.Set<UrlHistory>().FirstOrDefault(x => x.Site == site && x.Webpage == page && x.UrlSegment == url);
        }
    }
}