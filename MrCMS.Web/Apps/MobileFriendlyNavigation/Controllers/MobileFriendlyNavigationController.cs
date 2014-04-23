using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Controllers
{
    public class MobileFriendlyNavigationController : MrCMSAppUIController<MobileFriendlyNavigationApp>
    {
        private readonly IMobileFriendlyNavigationService _navigationService;

        public MobileFriendlyNavigationController(IMobileFriendlyNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [HttpGet]
        public ActionResult GetChildNodes(int parentId)
        {
            var data = _navigationService
                .GetChildNodes(parentId)
                .Select(x => new
                {
                    id = x.Id,
                    text = x.Text.ToString(),
                    url = x.Url.ToString(),
                    hasChildren = x.HasChildren
                });

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = data
            };
        }
    }
}