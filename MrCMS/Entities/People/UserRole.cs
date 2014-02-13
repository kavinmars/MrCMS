using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.ACL;
using MrCMS.Entities.Documents.Web;
using IRole = Microsoft.AspNet.Identity.IRole;

namespace MrCMS.Entities.People
{
    public class UserRole : SystemEntity, IRole
    {
        public UserRole()
        {
            FrontEndWebpages = new MrCMSSet<Webpage>();
            ACLRoles = new MrCMSList<ACLRole>();
            Users = new MrCMSSet<User>();
        }
        public const string Administrator = "Administrator";

        string IRole.Id
        {
            get { return OwinId; }
        }

        public virtual string OwinId
        {
            get { return Id.ToString(); }
        }

        [Required]
        [DisplayName("Role Name")]
        public virtual string Name { get; set; }

        public virtual MrCMSSet<User> Users { get; set; }

        public virtual bool IsAdmin { get { return Name == Administrator; } }

        public virtual MrCMSSet<Webpage> FrontEndWebpages { get; set; }
        public virtual MrCMSList<ACLRole> ACLRoles { get; set; }
    }
}
