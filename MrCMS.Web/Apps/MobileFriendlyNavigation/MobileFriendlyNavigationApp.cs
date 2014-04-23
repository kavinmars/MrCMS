using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Controllers;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "MobileFriendlyNavigation"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            //context.MapAreaRoute("Navigation controllers", "", "Apps/MobileFriendlyNavigation/{controller}/{action}/{id}",
            //    new {controller = "Home", action = "Index", id = UrlParameter.Optional},
            //    new[] {typeof (MobileFriendlyNavigationController).Namespace});

            context.MapRoute("MFNav GetChildren", "MobileFriendlyNavigation/GetChildNodes", new { controller = "MobileFriendlyNavigation", action = "GetChildNodes" });
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
        }
    }
}