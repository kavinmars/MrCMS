using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.DependencyResolution;
using MrCMS.Website;
using Ninject;

namespace MrCMS.DataAccess
{
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
}