using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.SQLite;
using System.Data.SqlClient;
using MrCMS.Website;
using MySql.Data.MySqlClient;
using Ninject;

namespace MrCMS.Helpers
{
    public interface IDbConfiguration
    {
        DbConnection GetDbConnection();
    }

    public class MrCMSDbConfig : DbConfiguration
    {
        public MrCMSDbConfig()
        {
            this.AddDependencyResolver(new MrCMSDbDependencyResolver());
        }
    }

    public class MrCMSDbDependencyResolver : IDbDependencyResolver
    {
        public object GetService(Type type, object key)
        {
            try
            {
                var kernel = MrCMSApplication.Get<IKernel>();
                return kernel.Get(type);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            try
            {
                return MrCMSApplication.Get<IKernel>().GetAll(type);
            }
            catch
            {
                return null;
            }
        }
    }
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