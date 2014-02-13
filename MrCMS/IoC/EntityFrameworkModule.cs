using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer;
using System.Security.AccessControl;
using MrCMS.DataAccess;
using MrCMS.Helpers;
using MrCMS.Migrations;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    public class EntityFrameworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseInitializer<MrCMSDbContext>>()
                .ToMethod(context => new MigrateDatabaseToLatestVersion<MrCMSDbContext, Configuration>())
                .InRequestScope();
            //Bind<Func<MigrationSqlGenerator>>()
            //    .ToMethod(context => () => new SqlServerMigrationSqlGenerator())
            //    .InRequestScope()
            //    .Named("System.Data.SqlClient");
            //Bind<DbProviderServices>()
            //    .ToMethod(context => SqlProviderServices.Instance)
            //    .InSingletonScope()
            //    .Named("System.Data.SqlClient");
            Bind<IDbConnectionFactory>().ToMethod(context
                =>
                 new MrCMSConnectionFactory()
                ).InSingletonScope();
            //Bind<IManifestTokenResolver>().ToMethod(context => new DefaultManifestTokenResolver()).InSingletonScope();
            //Bind<IDbProviderFactoryResolver>()
            //    .ToMethod(context => new MrCMSProviderFactoryResolver())
            //    .InSingletonScope();
            Bind<IDbExecutionStrategy>().ToMethod(context => new MrCMSExecutionStrategy()).InRequestScope();
            //Bind<DbSpatialServices>().ToMethod(context => DbSpatialServices.Default).InSingletonScope();
            //Bind<Func<DbConnection, string, HistoryContext>>().ToMethod(context => ((connection, s) => new HistoryContext(connection, s))).InRequestScope();
            Bind<IPluralizationService>().To<UnpluralizedService>().InRequestScope();
            Rebind<IDbContext>().ToMethod(context => context.Kernel.Get<StandardDbContext>()).InRequestScope();
        }
    }
}