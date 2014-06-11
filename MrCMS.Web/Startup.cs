using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using MrCMS.Helpers;
using MrCMS.Web;
using MrCMS.Website;
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

            app.ConfigureAuth();

            app.MapSignalR();
        }
    }

    public class MrCMSHubActivator : IHubActivator
    {
        public IHub Create(HubDescriptor descriptor)
        {
            return MrCMSApplication.Get(descriptor.HubType) as IHub;
        }
    }
}