using System.Web.Mvc;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Controllers
{
    public class MobileFriendlyNavigationController : MrCMSController
    {
        private readonly IMobileFriendlyNavigationService _navigationService;

        public MobileFriendlyNavigationController(IMobileFriendlyNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public JsonResult GetChildNodes(int parentId)
        {
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = _navigationService.GetChildNodes(parentId)
            };
        }
    }
}