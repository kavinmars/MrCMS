using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Galleries.Pages;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Galleries.Services
{
    public class GalleryListUIService : IGalleryListUIService
    {
        private readonly ISession _session;

        public GalleryListUIService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Gallery> GetGalleries(GalleryList page, string category, int p)
        {

            return _session.QueryOver<Gallery>().Where(
                gallery =>
                    gallery.Parent == page && gallery.PublishOn != null &&
                    gallery.PublishOn <= CurrentRequestData.Now)
                .ThenBy(gallery => gallery.DisplayOrder)
                .Desc.Paged(p, page.PageSize);

        }
    }
}