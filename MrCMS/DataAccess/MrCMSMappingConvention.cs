using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace MrCMS.DataAccess
{
    public class MrCMSMappingConvention : Convention
    {
        public MrCMSMappingConvention()
        {
            this.Properties()
                .Configure(configuration => configuration.HasColumnName(configuration.ClrPropertyInfo.Name));
        }
    }
}