using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using Ninject;

namespace MrCMS.Web.Apps.Articles
{
    public class ArticlesApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Articles"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            
        }

        protected override void OnInstallation(IDbContext dbContext, InstallModel model, Site site)
        {
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }
}