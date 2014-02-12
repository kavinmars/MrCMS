using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public class UpdateUrlHistoryService : IUpdateUrlHistoryService
    {
        private readonly IDbContext _session;
        private readonly Site _site;
        private HashSet<UrlHistory> _urlHistories;

        public UpdateUrlHistoryService(IDbContext session, Site site)
        {
            _session = session;
            _site = site;
        }

        public HashSet<UrlHistory> UrlHistories
        {
            get { return _urlHistories; }
        }

        public IUpdateUrlHistoryService Initialise()
        {
            _urlHistories = new HashSet<UrlHistory>(_session.Query<UrlHistory>().Where(history => history.Site.Id == _site.Id));
            return this;
        }

        public void SetUrlHistory(DocumentImportDTO documentDto, Webpage webpage)
        {
            var urlsToAdd = documentDto.UrlHistory.Where(s => !webpage.Urls.Select(history => history.UrlSegment).Contains(s, StringComparer.InvariantCultureIgnoreCase)).ToList();
            var urlsToRemove = webpage.Urls.Where(history => !documentDto.UrlHistory.Contains(history.UrlSegment, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in urlsToAdd)
            {
                var history = UrlHistories.FirstOrDefault(t => t.UrlSegment.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                if (history == null)
                {
                    history = new UrlHistory { UrlSegment = item, Webpage = webpage };
                    UrlHistories.Add(history);
                }
                else
                    history.Webpage = webpage;
                if (!webpage.Urls.Contains(history))
                    webpage.Urls.Add(history);
            }

            foreach (var history in urlsToRemove)
            {
                webpage.Urls.Remove(history);
                history.Webpage = null;
            }
        }

        public void SaveUrlHistories()
        {
            _session.Transact(dbContext =>
                                  {
                                      foreach (var urlHistory in UrlHistories)
                                      {
                                          if (urlHistory.Webpage == null)
                                              dbContext.Delete(urlHistory);
                                          else
                                              dbContext.AddOrUpdate(urlHistory);
                                      }
                                  });
        }
    }
}