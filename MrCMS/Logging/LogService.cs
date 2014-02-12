using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using MrCMS.DataAccess;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Settings;

namespace MrCMS.Logging
{
    public class LogService : ILogService
    {
        private readonly IDbContext _dbContext;
        private readonly SiteSettings _siteSettings;

        public LogService(IDbContext dbContext, SiteSettings siteSettings)
        {
            _dbContext = dbContext;
            _siteSettings = siteSettings;
        }

        public void Insert(Log log)
        {
            if (log.Error == null)
                log.Error = new Error();
            log.Guid = Guid.NewGuid();
            _dbContext.Transact(session => session.Add(log));
        }

        public IList<Log> GetAllLogEntries()
        {
            return BaseQuery().ToList();
        }

        public void DeleteAllLogs()
        {
            _dbContext.Database.ExecuteSqlCommand("delete Log l");
        }

        public void DeleteLog(Log log)
        {
            _dbContext.Transact(session => session.Delete(log));
        }

        public List<SelectListItem> GetSiteOptions()
        {
            var sites = _dbContext.Query<Site>().OrderBy(site => site.Name).ToList();
            return sites.Count == 1
                       ? new List<SelectListItem>()
                       : sites.BuildSelectItemList(site => site.Name, site => site.Id.ToString(), emptyItemText: "All sites");
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            var query = BaseQuery();
            if (searchQuery.Type.HasValue)
                query = query.Where(log => log.Type == searchQuery.Type);

            if (!string.IsNullOrWhiteSpace(searchQuery.Message))
                query =
                    query.Where(
                        log =>
                        log.Message.Contains(searchQuery.Message));

            if (!string.IsNullOrWhiteSpace(searchQuery.Detail))
                query = query.Where(log => log.Detail.Contains(searchQuery.Detail));

            if (searchQuery.SiteId.HasValue)
                query = query.Where(log => log.Site.Id == searchQuery.SiteId);

            if (searchQuery.From.HasValue)
                query = query.Where(log => log.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                query = query.Where(log => log.CreatedOn <= searchQuery.To);

            return query.Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        private IQueryable<Log> BaseQuery()
        {
            return
                _dbContext.Query<Log>()
                          .OrderByDescending(entry => entry.Id);
        }
    }
}