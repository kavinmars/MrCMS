using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Core.Widgets
{
    public class LinkedImage : Widget 
    {
        public virtual string Image { get; set; }
        public virtual string Link { get; set; }
    }
}