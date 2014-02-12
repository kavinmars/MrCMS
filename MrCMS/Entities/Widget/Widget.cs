using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Entities.Widget
{
    public abstract class Widget : SiteEntity
    {
        protected Widget()
        {
            ShownOn = new HashSet<Webpage>();
            HiddenOn = new HashSet<Webpage>();
        }
        public virtual LayoutArea LayoutArea { get; set; }

        public virtual string Name { get; set; }

        [DisplayName("Custom Layout (leave blank to use default)")]
        public virtual string CustomLayout { get; set; }

        public virtual string WidgetTypeFormatted { get { return ObjectTypeName.BreakUpString(); } }

        public virtual Webpage Webpage { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual string Name2 { get; set; }

        [DefaultValue(true)]
        [DisplayName("Show on child pages")]
        public virtual bool IsRecursive { get; set; }

        public virtual IList<PageWidgetSort> PageWidgetSorts { get; set; }
	
        public virtual object GetModel(IKernel kernel)
        {
            return this;
        }

        public virtual ISet<Webpage> HiddenOn { get; set; }
        public virtual ISet<Webpage> ShownOn { get; set; }

        public virtual void SetDropdownData(ViewDataDictionary viewData, IKernel kernel) { }

        public virtual bool HasProperties { get { return true; } }

        public override void OnDeleting(IDbContext dbContext)
        {
            ShownOn.ForEach(webpage => webpage.ShownWidgets.Remove(this));
            HiddenOn.ForEach(webpage => webpage.HiddenWidgets.Remove(this));
            if (LayoutArea != null) LayoutArea.Widgets.Remove(this); //required to clear cache
            if (Webpage != null)
            {
                Webpage.Widgets.Remove(this);
            }
            base.OnDeleting(dbContext);
        }
    }
}
