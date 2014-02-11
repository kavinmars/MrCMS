using System.Web;
using MrCMS.Website;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    public class ContextModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<HttpContextBase>()
                .ToMethod(context => new HttpContextWrapper(HttpContext.Current))
                .When(request => HttpContext.Current != null)
                .InRequestScope();
            Kernel.Bind<HttpContextBase>()
                .ToMethod(context => new OutOfContext())
                .When(request => HttpContext.Current == null)
                .InThreadScope();
        }
    }
}