using System.Threading;
using System.Threading.Tasks;
using Elmah;
using FluentNHibernate.Utils;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web;
using MrCMS.Website;
using Ninject;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace MrCMS.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubActivator = new MrCMSHubActivator();

            GlobalHost.DependencyResolver.Register(
                typeof (IHubActivator),
                () => hubActivator);
            
            app.Use<KernelCreator>();
            app.Use<MrCMSLifecyleSetter>();
            app.UseRequestScopeContext();

            app.ConfigureAuth();

            app.MapSignalR();
        }
    }
    public class MrCMSLifecyleSetter : OwinMiddleware 
    {
        public MrCMSLifecyleSetter(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            IKernel kernel = context.GetKernel();
            CurrentRequestData.ErrorSignal = ErrorSignal.FromCurrentContext();
            CurrentRequestData.CurrentSite = kernel.Get<ICurrentSiteLocator>().GetCurrentSite();
            CurrentRequestData.SiteSettings = kernel.Get<SiteSettings>();
            CurrentRequestData.HomePage = kernel.Get<IDocumentService>().GetHomePage();
            Thread.CurrentThread.CurrentCulture = CurrentRequestData.SiteSettings.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = CurrentRequestData.SiteSettings.CultureInfo;
            await Next.Invoke(context);
        }
    }

    public class MrCMSHubActivator : IHubActivator
    {
        public IHub Create(HubDescriptor descriptor)
        {
            return KernelCreator.GetNew().Get(descriptor.HubType) as IHub;
        }
    }
}