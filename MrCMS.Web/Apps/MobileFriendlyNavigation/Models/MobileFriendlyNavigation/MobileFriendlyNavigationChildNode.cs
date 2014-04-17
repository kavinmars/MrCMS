using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationRootNode
    {
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public IEnumerable<MobileFriendlyNavigationChildNode> Children { get; set; }

        public bool HasChildren
        {
            get { return Children.Any(); }
        }

        public HtmlString Url
        {
            get { return new HtmlString("/" + UrlSegment); }
        }

        public HtmlString Text
        {
            get { return new HtmlString(Name); }
        }
    }

    public class MobileFriendlyNavigationChildNode
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public int ChildCount { get; set; }

        public bool HasChildren
        {
            get { return ChildCount > 0; }
        }

        public HtmlString Url
        {
            get { return new HtmlString("/" + UrlSegment); }
        }

        public HtmlString Text
        {
            get { return new HtmlString(Name); }
        }
    }
}