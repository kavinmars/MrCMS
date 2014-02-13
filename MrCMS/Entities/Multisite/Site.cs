using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Website;
using System.Linq;

namespace MrCMS.Entities.Multisite
{
    public class Site : SystemEntity
    {
        public Site()
        {
            RedirectedDomains = new MrCMSList<RedirectedDomain>();
        }
        [Required]
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        [Required]
        public virtual string BaseUrl { get; set; }

        public virtual MrCMSList<RedirectedDomain> RedirectedDomains { get; set; }

        public virtual string DisplayName
        {
            get { return string.Format("{0} ({1})", Name, BaseUrl); }
        }

        public virtual bool IsValidForSite(SiteEntity entity)
        {
            if (entity.ObjectType.GetCustomAttributes(typeof(AdminUISiteAgnosticAttribute), true).Any())
                return true;
            return entity.Site != null && entity.Site.Id == Id;
        }
    }
}
