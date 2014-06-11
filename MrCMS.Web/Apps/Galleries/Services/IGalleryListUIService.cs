using MrCMS.Paging;
using MrCMS.Web.Apps.Galleries.Pages;

namespace MrCMS.Web.Apps.Galleries.Services
{
    public interface IGalleryListUIService
    {
        IPagedList<Gallery> GetGalleries(GalleryList page, string category, int p);
    }
}