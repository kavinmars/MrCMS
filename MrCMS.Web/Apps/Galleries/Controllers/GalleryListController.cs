using System.Web.Mvc;
using MrCMS.Web.Apps.Galleries.Pages;
using MrCMS.Web.Apps.Galleries.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Galleries.Controllers
{
    public class GalleryListController : MrCMSAppUIController<GalleriesApp>
    {
        private readonly IGalleryListUIService _galleryListService;

        public GalleryListController(IGalleryListUIService galleryListService)
        {
            _galleryListService = galleryListService;
        }

        public ActionResult Show(GalleryList page, string category, int p = 1)
        {
            ViewData["galleries"] = _galleryListService.GetGalleries(page, category, p);

            return View(page);
        }
    }
}