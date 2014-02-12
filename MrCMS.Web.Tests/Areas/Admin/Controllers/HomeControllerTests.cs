using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.DataAccess;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class HomeControllerTests
    {
        [Fact(Skip = "needs moving into service")]
        public void HomeController_OnGetIndex_ReturnsAViewResult()
        {
            var currentSiteLocator = A.Fake<ICurrentSiteLocator>();
            var userService = A.Fake<IUserService>();
            var session = A.Fake<IDbContext>();
            var homeController = new HomeController(currentSiteLocator, userService, session);

            ActionResult actionResult = homeController.Index();

            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<ViewResult>();
        }
    }
}