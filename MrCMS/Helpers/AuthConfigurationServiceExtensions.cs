using MrCMS.Services;
using MrCMS.Website;
using Ninject;
using Owin;

namespace MrCMS.Helpers
{
    public static class AuthConfigurationServiceExtensions
    {

        public static void ConfigureAuth(this IAppBuilder app)
        {
            //IKernel kernel = KernelCreator.Kernel;
            //var standardAuthConfigurationService = kernel.Get<IStandardAuthConfigurationService>();
            //standardAuthConfigurationService.ConfigureAuth(app);
            //if (CurrentRequestData.DatabaseIsInstalled)
            //{
            //    var authConfigurationService = kernel.Get<IAuthConfigurationService>();
            //    authConfigurationService.ConfigureAuth(app);
            //}
        }
    }
}