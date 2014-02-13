using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MrCMS.DataAccess.Mappings;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.DataAccess
{
    public class MrCMSDbContext : DbContext
    {
        private readonly IEnumerable<IPreCommitListener> _preCommitListeners;

        public MrCMSDbContext(IEnumerable<IPreCommitListener> preCommitListeners)
        {
            _preCommitListeners = preCommitListeners;
        }

        public override int SaveChanges()
        {
            _preCommitListeners.ForEach(listener => listener.OnPreCommit(ChangeTracker));
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            var toMap = GetTypesToMap();
            foreach (var type in toMap)
            {
                DbMappings.Mappers[type].Map(modelBuilder);
            }
            modelBuilder.Conventions.Add(new MrCMSMappingConvention(), new ForeignKeyNamingConvention());
        }

        public static List<Type> GetTypesToMap()
        {
            List<Type> allClasses =
                EnumerableHelper.ToList(TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>())
                    .FindAll(type => type != typeof(SystemEntity) && type != typeof(SiteEntity));
            return allClasses;
        }
    }
}