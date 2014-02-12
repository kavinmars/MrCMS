using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WebpageQueryingExtensions
    {
        public static IQueryable<T> Published<T>(this IQueryable<T> webpages) where T : Webpage
        {
            return webpages.Where(webpage => webpage.PublishOn != null && webpage.PublishOn <= CurrentRequestData.Now);
        }
    }
}