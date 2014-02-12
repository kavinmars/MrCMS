using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using MrCMS.DataAccess;

namespace MrCMS.Entities
{
    public abstract class SystemEntity
    {
        public virtual int Id { get; set; }
        [DisplayName("Created On")]
        public virtual DateTime CreatedOn { get; set; }
        [DisplayName("Updated On")]
        public virtual DateTime UpdatedOn { get; set; }

        [NotMapped]
        public virtual bool IsDeleted { get; set; }

        public virtual void OnDeleting(IDbContext dbContext)
        {
        }
        public Type ObjectType
        {
            get
            {
                var type = GetType();
                if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
                {
                    type = type.BaseType;
                }
                return type;
            }
        }

        public virtual string ObjectTypeName
        {
            get
            {
                return ObjectType.Name;
            }
        }
    }
}