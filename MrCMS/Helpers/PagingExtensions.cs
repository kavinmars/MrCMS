using System.Linq;
using MrCMS.Entities;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class PagingExtensions
    {
        public static IPagedList<T> Paged<T>(this IQueryable<T> query, int page, int? pageSize = null) where T : SystemEntity
        {
            return new PagedList<T>(query, page, pageSize ?? MrCMSApplication.Get<SiteSettings>().DefaultPageSize);
        }
    }
}