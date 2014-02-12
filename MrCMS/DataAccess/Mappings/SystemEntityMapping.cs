using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using MrCMS.Entities;

namespace MrCMS.DataAccess.Mappings
{
    public abstract class SystemEntityMapping<T> : EntityTypeConfiguration<T>, IMapDbModel where T : SystemEntity
    {
        protected SystemEntityMapping()
        {
        }
        public void Map(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Configurations.Add(this);
        }
        public virtual Type BaseType { get { return typeof(SystemEntity); } }

        public int Specificity
        {
            get
            {
                var type = typeof(T);
                var count = 0;
                while (type != null && type != BaseType)
                {
                    type = type.BaseType;
                    count++;
                }
                return count;
            }
        }
    }
}