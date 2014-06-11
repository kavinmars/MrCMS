using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin;
using MrCMS.Website;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace MrCMS.Helpers
{
    public static class IoCExtensions
    {
        public static IKernel GetKernel(this RequestContext requestContext)
        {
            if (requestContext != null)
            {
                return GetKernel(requestContext.HttpContext);
            }
            return GetNewChildKernel();
        }

        public static IKernel GetKernel(this HttpContextBase httpContext)
        {
            if (httpContext != null)
            {
                IOwinContext owinContext = httpContext.GetOwinContext();
                if (owinContext != null)
                    return owinContext.Get<IKernel>(KernelCreator.ContextKey);
            }
            return GetNewChildKernel();
        }

        public static T Get<T>(this HttpContextBase httpContext)
        {
            return GetKernel(httpContext).Get<T>();
        }

        public static IEnumerable<T> GetAll<T>(this HttpContextBase httpContext)
        {
            return GetKernel(httpContext).GetAll<T>();
        }

        public static IKernel GetKernel(this HttpContext httpContext)
        {
            if (httpContext != null)
            {
                IOwinContext owinContext = httpContext.GetOwinContext();
                if (owinContext != null)
                    return owinContext.Get<IKernel>(KernelCreator.ContextKey);
            }
            return GetNewChildKernel();
        }

        public static IKernel GetKernel(this HtmlHelper htmlHelper)
        {
            if (htmlHelper != null && htmlHelper.ViewContext != null)
            {
                return GetKernel(htmlHelper.ViewContext.HttpContext);
            }
            return GetNewChildKernel();
        }
        public static T Get<T>(this HtmlHelper htmlHelper)
        {
            return GetKernel(htmlHelper).Get<T>();
        }
        public static IEnumerable<T> GetAll<T>(this HtmlHelper htmlHelper)
        {
            return GetKernel(htmlHelper).GetAll<T>();
        }

        private static ChildKernel GetNewChildKernel()
        {
            return new ChildKernel(KernelCreator.Kernel);
        }
    }
}