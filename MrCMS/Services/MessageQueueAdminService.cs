using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class MessageQueueAdminService : IMessageQueueAdminService
    {
        private readonly IDbContext _dbContext;
        private readonly SiteSettings _siteSettings;
        private readonly Site _site;

        public MessageQueueAdminService(IDbContext dbContext, SiteSettings siteSettings, Site site)
        {
            _dbContext = dbContext;
            _siteSettings = siteSettings;
            _site = site;
        }

        public IPagedList<QueuedMessage> GetMessages(MessageQueueQuery searchQuery)
        {
            var queryOver = _dbContext.Query<QueuedMessage>().Where(message => message.Site.Id == _site.Id);
            if (searchQuery.From.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                queryOver = queryOver.Where(message => message.CreatedOn <= searchQuery.To);
            if (!string.IsNullOrWhiteSpace(searchQuery.FromQuery))
                queryOver =
                    queryOver.Where(message => message.FromAddress.Contains(searchQuery.FromQuery) || message.FromName.Contains(searchQuery.FromQuery));
            if (!string.IsNullOrWhiteSpace(searchQuery.ToQuery))
                queryOver =
                    queryOver.Where(message => message.ToAddress.Contains(searchQuery.ToQuery) || message.ToName.Contains(searchQuery.ToQuery));

            return queryOver.OrderByDescending(message => message.CreatedOn)
                            .Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }
    }
}