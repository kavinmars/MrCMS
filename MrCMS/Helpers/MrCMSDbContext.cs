using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using MrCMS.DataAccess.Mappings;
using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public class MrCMSDbContext : DbContext
    {
        public override int SaveChanges()
        {
            var createdOn = CurrentRequestData.Now;
            foreach (
                var entity in
                    this.ChangeTracker.Entries()
                        .Where(entry => entry.State == EntityState.Added)
                        .Select(entry => entry.Entity)
                        .OfType<SystemEntity>())
            {
                entity.CreatedOn = createdOn;
            }
            foreach (
                var entity in
                    this.ChangeTracker.Entries()
                        .Where(entry => (entry.State == EntityState.Added || entry.State == EntityState.Modified))
                        .Select(entry => entry.Entity)
                        .OfType<SystemEntity>())
            {
                entity.UpdatedOn = createdOn;
                if (entity is SiteEntity)
                {
                    (entity as SiteEntity).Site = CurrentRequestData.CurrentSite;
                }
            }
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
            modelBuilder.Conventions.Add(new StringNameConvention(), new ForeignKeyNamingConvention());
        }

        public static List<Type> GetTypesToMap()
        {
            List<Type> allClasses =
                TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>()
                    .ToList()
                    .FindAll(type => type != typeof(SystemEntity) && type != typeof(SiteEntity));
            return allClasses;
            //var systemTypes =
            //    allClasses.FindAll(type => type != typeof(SiteEntity) && type.BaseType == typeof(SystemEntity));
            //var siteTypes = allClasses.FindAll(type => type.BaseType == typeof(SiteEntity));
            //var toMap = systemTypes.Concat(siteTypes).ToList();
            //return toMap;
        }
    }

    public class StringNameConvention : Convention
    {
        public StringNameConvention()
        {
            this.Properties()
                .Configure(configuration =>
                           {
                               configuration.HasColumnName(configuration.ClrPropertyInfo.Name);
                           });
        }
    }

    //public class ColumnNameConvention : IStoreModelConvention<EntityType>, IStoreModelConvention<EdmProperty>
    //{
    //    public void Apply(EdmProperty item, DbModel model)
    //    {
    //        var name = item.Name;
    //        var preferred = item.MetadataProperties.FirstOrDefault(property => property.Name == "PreferredName");

    //        if (preferred != null && name != preferred.Value.ToString())
    //        {
    //            item.Name = preferred.Value.ToString();
    //        }

    //        //if(name != declaringTypeName)
    //        //{
    //        //    //item.Name = declaringTypeName;
    //        //}
    //    }

    //    public void Apply(EntityType item, DbModel model)
    //    {
    //        var groupBy = item.Properties.GroupBy(property =>
    //                                              {
    //                                                  var firstOrDefault = property.MetadataProperties.FirstOrDefault(metadataProperty => metadataProperty.Name == "PreferredName");
    //                                                  return firstOrDefault != null ? firstOrDefault.Value : null;
    //                                              });
    //        var duplicateGroups = groupBy.Where(grouping => grouping.Key != null && grouping.Count() > 1);

    //        foreach (var duplicateGroup in duplicateGroups)
    //        {
    //            var edmProperty = duplicateGroup.First();
    //        }
    //    }
    //}

    /// <summary>
    /// Provides a convention for fixing the independent association (IA) foreign key column names.
    /// </summary>
    public class ForeignKeyNamingConvention : IStoreModelConvention<AssociationType>
    {
        public void Apply(AssociationType association, DbModel model)
        {
            // Identify a ForeignKey properties (including IAs)
            if (association.IsForeignKey)
            {
                // rename FK columns
                var constraint = association.Constraint;
                if (DoPropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToRole.Name, constraint.ToProperties))
                {
                    NormalizeForeignKeyProperties(constraint.FromProperties);
                }
                if (DoPropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromRole.Name, constraint.FromProperties))
                {
                    NormalizeForeignKeyProperties(constraint.ToProperties);
                }
            }
        }

        private bool DoPropertiesHaveDefaultNames(ReadOnlyMetadataCollection<EdmProperty> properties, string roleName, ReadOnlyMetadataCollection<EdmProperty> otherEndProperties)
        {
            if (properties.Count != otherEndProperties.Count)
            {
                return false;
            }

            for (int i = 0; i < properties.Count; ++i)
            {
                if (!properties[i].Name.EndsWith("_" + otherEndProperties[i].Name))
                {
                    return false;
                }
            }
            return true;
        }

        private void NormalizeForeignKeyProperties(ReadOnlyMetadataCollection<EdmProperty> properties)
        {
            for (int i = 0; i < properties.Count; ++i)
            {
                string defaultPropertyName = properties[i].Name;
                int ichUnderscore = defaultPropertyName.IndexOf('_');
                if (ichUnderscore <= 0)
                {
                    continue;
                }
                string navigationPropertyName = defaultPropertyName.Substring(0, ichUnderscore);
                string targetKey = defaultPropertyName.Substring(ichUnderscore + 1);

                string newPropertyName;
                if (targetKey.StartsWith(navigationPropertyName))
                {
                    newPropertyName = targetKey;
                }
                else
                {
                    newPropertyName = navigationPropertyName + targetKey;
                }
                properties[i].Name = newPropertyName;
            }
        }

    }
}