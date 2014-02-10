using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Installation;
using Ninject;

namespace MrCMS.Web.Apps.Core
{
    public class CoreApp : MrCMSApp
    {
        private readonly CoreAppInstallation _coreAppInstallation = new CoreAppInstallation();

        public override string AppName
        {
            get { return "Core"; }
        }

        protected override int InstallOrder
        {
            get { return 1; }
        }

        protected override void RegisterServices(IKernel kernel)
        {

        }

        protected override void OnInstallation(IDbContext dbContext, InstallModel model, Site site)
        {
            CoreAppInstallation.Install(dbContext, model, site);
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("External Login", "external-login", new { controller = "ExternalLogin", action = "Login" });
            context.MapRoute("External Login Callback", "external-login/callback", new { controller = "ExternalLogin", action = "Callback" });

            context.MapRoute("User Registration", "Registration/RegistrationDetails", new { controller = "Registration", action = "RegistrationDetails" });
            context.MapRoute("User Registration - check email", "Registration/CheckEmailIsNotRegistered", new { controller = "Registration", action = "CheckEmailIsNotRegistered" });

            context.MapRoute("UserAccountController - account details", "UserAccount/UserAccountDetails", new { controller = "UserAccount", action = "UserAccountDetails" });
            context.MapRoute("UserAccountController - check email isn't already registered", "UserAccount/IsUniqueEmail", new { controller = "UserAccount", action = "IsUniqueEmail" });

            context.MapRoute("UserAccountController - change password", "UserAccount/ChangePassword", new { controller = "UserAccount", action = "ChangePassword" });
        }
    }
}