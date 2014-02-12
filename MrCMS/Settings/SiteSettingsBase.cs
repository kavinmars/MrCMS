using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Settings
{
    public abstract class SiteSettingsBase
    {
        public Site Site { get; set; }

        public virtual string TypeName
        {
            get { return GetType().Name.BreakUpString(); }
        }

        public virtual string DivId
        {
            get { return GetType().Name.BreakUpString().ToLowerInvariant().Replace(" ", "-"); }
        }

        public virtual bool RenderInSettings
        {
            get { return false; }
        }

        public virtual void SetViewData(IDbContext dbContext, ViewDataDictionary viewDataDictionary)
        {

        }
    }
}