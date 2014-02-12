using System.Linq;
using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class HomeController : MrCMSAdminController
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly IUserService _userServices;
        private readonly IDbContext _dbContext;

        public HomeController(ICurrentSiteLocator currentSiteLocator, IUserService userServices, IDbContext dbContext)
        {
            _currentSiteLocator = currentSiteLocator;
            _userServices = userServices;
            _dbContext = dbContext;
        }

        public ActionResult Index()
        {
            var currentSite = _currentSiteLocator.GetCurrentSite();
            var list = _dbContext.Query<Webpage>()
                                 .Where(x => x.Site.Id == currentSite.Id).ToList()
                                 .GroupBy(webpage => webpage.ObjectTypeName)
                                 .Select(webpages => new WebpageStats
                                     {
                                         DocumentType = webpages.Key,
                                         NumberOfPages = webpages.Count(),
                                         NumberOfUnPublishedPages = webpages.Count(webpage => !webpage.Published)
                                     }).ToList();

            var model = new Dashboard
                            {
                                SiteName = currentSite.Name.Trim(),
                                LoggedInName = _userServices.GetCurrentUser(HttpContext).FirstName,
                                Stats = list,
                                ActiveUsers = _userServices.ActiveUsers(),
                                NoneActiveUsers = _userServices.NonActiveUsers()
                            };

            return View(model);
        }
    }
}