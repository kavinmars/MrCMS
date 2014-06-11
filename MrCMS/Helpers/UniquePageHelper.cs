using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class UniquePageHelper
    {
        public static string GetUrl<T>(this HttpContextBase context, object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(context, queryString, arg => "/" + arg.LiveUrlSegment);
        }

        public static string GetAbsoluteUrl<T>(this HttpContextBase context, object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(context, queryString, arg => arg.AbsoluteUrl);
        }
        public static string GetUrl<T>(this HtmlHelper helper, object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(helper.ViewContext.HttpContext, queryString, arg => "/" + arg.LiveUrlSegment);
        }

        public static string GetAbsoluteUrl<T>(this HtmlHelper helper, object queryString = null) where T : Webpage, IUniquePage
        {
            return Get<T>(helper.ViewContext.HttpContext, queryString, arg => arg.AbsoluteUrl);
        }

        private static string Get<T>(HttpContextBase helper, object queryString, Func<T, string> selector) where T : Webpage, IUniquePage
        {
            var service = helper.Get<IUniquePageService>();

            var processPage = service.GetUniquePage<T>();
            string url = processPage != null ? selector(processPage) : "/";
            if (queryString != null && processPage != null)
            {
                var dictionary = new RouteValueDictionary(queryString);
                url += string.Format("?{0}",
                                     string.Join("&",
                                                 dictionary.Select(
                                                     pair => string.Format("{0}={1}", pair.Key, pair.Value))));
            }
            return url;
        }
    }
}