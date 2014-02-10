using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.IoC;
using MrCMS.Settings;
using MrCMS.Website;
using Ninject;
using Ninject.MockingKernel;

namespace MrCMS.Web.Tests
{
    public abstract class MrCMSTest : IDisposable
    {
        protected MockingKernel Kernel;

        protected MrCMSTest()
        {
            Kernel = new MockingKernel();
            Kernel.Load(new ContextModule());
            MrCMSApplication.OverrideKernel(Kernel);
            CurrentRequestData.SiteSettings = new SiteSettings();
        }

        public virtual void Dispose()
        {
        }
    }
}
