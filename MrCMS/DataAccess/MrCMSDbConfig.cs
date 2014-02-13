using System.Data.Entity;

namespace MrCMS.DataAccess
{
    public class MrCMSDbConfig : DbConfiguration
    {
        public MrCMSDbConfig()
        {
            this.AddDependencyResolver(new MrCMSDbDependencyResolver());
        }
    }
}