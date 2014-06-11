using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.IoC;
using MrCMS.Services;
using NHibernate;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Web.Common;

namespace MrCMS.Website
{
    public class KernelCreator : OwinMiddleware
    {
        private static readonly IKernel _kernel;

        static KernelCreator()
        {

            _kernel = new StandardKernel(new ServiceModule(),
                new NHibernateModule(DatabaseType.Auto));
            _kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            _kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

        }
        public const string ContextKey = "current.ninject.kernel";
        public const string EventContextKey = "event.context";

        public KernelCreator(OwinMiddleware next)
            : base(next)
        {
        }

        public static IKernel Kernel { get { return _kernel; } }

        public override async Task Invoke(IOwinContext context)
        {
            using (var childKernel = CreateChildKernel())
            {
                childKernel.Bind<IOwinContext>().ToConstant(context).InSingletonScope();
                childKernel.Rebind<IKernel>().ToConstant(childKernel).InSingletonScope();
                childKernel.Bind<Site>().ToConstant(childKernel.Get<ICurrentSiteLocator>().GetCurrentSite());

                context.Set(ContextKey, childKernel);
                context.Set(EventContextKey, childKernel.Get<IEventContext>());
                await Next.Invoke(context);
            }
        }

        public static IKernel GetNew()
        {
            return CreateChildKernel();
        }

        private static ChildKernel CreateChildKernel()
        {
            return new ChildKernel(_kernel);
        }
    }
}