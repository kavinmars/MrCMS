using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.SqlClient;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        private readonly MockingKernel _kernel;

        protected MrCMSTest()
        {
            _kernel = new MockingKernel();
            Kernel.Load(new ContextModule());
            MrCMSApplication.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public MockingKernel Kernel
        {
            get { return _kernel; }
        }

        public virtual void Dispose()
        {
        }
    }
    public class MiscTests
    {
        [Fact]
        public void Factories()
        {
            var sqlConnection = new SqlConnection(@"Data Source=.\sqlexpress;Initial Catalog=mrcms-test;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True");
            DbProviderServices dbProviderServices =
                System.Data.Entity.Core.Common.DbProviderServices.GetProviderServices(
                    sqlConnection);

            string s = dbProviderServices.ToString();
            string providerManifestToken = dbProviderServices.GetProviderManifestToken(sqlConnection);
            DbProviderManifest dbProviderManifest = dbProviderServices.GetProviderManifest(providerManifestToken);
        }
    }
}