using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MrCMS.Helpers
{
    public interface IDbConfiguration
    {
        DbConnection GetDbConnection();
    }

    public class DbConfiguration : IDbConfiguration
    {
        public static DbConnection Override { get; set; }
        public DbConnection GetDbConnection()
        {
            if (Override != null)
                return Override;
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["mrcms"];
            DbConnection connection = null;
            switch (connectionStringSettings.ProviderName)
            {
                case "System.Data.SQLite":
                    connection = new SQLiteConnection(connectionStringSettings.ConnectionString);
                    break;
                case "System.Data.SqlClient":
                    connection = new SqlConnection(connectionStringSettings.ConnectionString);
                    break;
                case "MySql.Data.MySqlClient":
                    connection = new MySqlConnection(connectionStringSettings.ConnectionString);
                    break;
            }
            //return new EntityConnection(connectionStringSettings.ConnectionString)
            return connection;
        }
    }
}