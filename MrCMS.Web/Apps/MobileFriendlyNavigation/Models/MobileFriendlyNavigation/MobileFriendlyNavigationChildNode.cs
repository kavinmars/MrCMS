namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationChildNode
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public int ChildCount { get; set; }

        public object ToJson()
        {
            return new
            {
                id = Id,
                text = Name,
                url = (!UrlSegment.StartsWith("/") ? "/" : string.Empty) + UrlSegment,
                hasChildren = ChildCount > 0
            };
        }
    }
}