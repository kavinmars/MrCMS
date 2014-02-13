using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace MrCMS.DataAccess
{
    public class MrCMSConnectionFactory : IDbConnectionFactory
    {
        public static string OverrideConnectionString { get; set; }
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            if (!string.IsNullOrWhiteSpace(OverrideConnectionString))
            {
                return new SqlConnection(OverrideConnectionString);
            }
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["mrcms"];
            return new SqlConnection(connectionStringSettings.ConnectionString);
        }
    }

    
}