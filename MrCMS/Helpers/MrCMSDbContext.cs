using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using MrCMS.Entities;

namespace MrCMS.Helpers
{
    public class MrCMSDbContext : DbContext
    {
        public MrCMSDbContext(IDbConfiguration configuration)
            : base(configuration.GetDbConnection(), true)
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var toMap = GetTypesToMap();
            MethodInfo methodInfo = typeof(DbModelBuilder).GetMethodExt("Entity");
            foreach (var type in toMap)
            {
                methodInfo.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { });
            }
        }

        public static List<Type> GetTypesToMap()
        {
            List<Type> allClasses = TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>().ToList();
            var systemTypes =
                allClasses.FindAll(type => type != typeof(SiteEntity) && type.BaseType == typeof(SystemEntity));
            var siteTypes = allClasses.FindAll(type => type.BaseType == typeof(SiteEntity));
            var toMap = systemTypes.Concat(siteTypes).ToList();
            return toMap;
        }
    }
}