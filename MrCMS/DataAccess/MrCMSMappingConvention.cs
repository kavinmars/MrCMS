using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.DataAccess
{
    public class MrCMSMappingConvention : Convention
    {
        public MrCMSMappingConvention()
        {
            this.Properties()
                .Configure(configuration => configuration.HasColumnName(configuration.ClrPropertyInfo.Name));
            this.Types()
                .Where(type => type.GetCustomAttributes(typeof(DoNotMapAttribute), true).Any())
                .Configure(configuration => configuration.Ignore());
        }
    }
}