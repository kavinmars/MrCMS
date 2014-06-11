using System.Web.Mvc;
using System.Web.WebPages;
using MrCMS.Website;
using MrCMS.Website.Optimization;
using Ninject;

namespace MrCMS.Helpers
{
    public static class OptimisationExtensions
    {
        public static void IncludeScript(this HtmlHelper helper, string url)
        {
            var webPage = helper.ViewDataContainer as WebPageBase;
            var virtualPath = webPage == null ? string.Empty : webPage.VirtualPath;
            helper.Get<IResourceBundler>().AddScript(virtualPath, url);
        }

        public static MvcHtmlString RenderScripts(this HtmlHelper helper)
        {
            return helper.Get<IResourceBundler>().GetScripts();
        }

        public static void IncludeCss(this HtmlHelper helper, string url)
        {
            var webPage = helper.ViewDataContainer as WebPageBase;
            var virtualPath = webPage == null ? string.Empty : webPage.VirtualPath;
            helper.Get<IResourceBundler>().AddCss(virtualPath, url);
        }

        public static MvcHtmlString RenderCss(this HtmlHelper helper)
        {
            return helper.Get<IResourceBundler>().GetCss();
        }
    }
}