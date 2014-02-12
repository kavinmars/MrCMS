using System;
using MrCMS.Entities.Documents;

namespace MrCMS.DataAccess.Mappings
{
    public class DocumentMapping : SystemEntityMapping<Document>
    {
        public DocumentMapping()
        {
            this.HasMany(document => document.Tags)
                .WithMany(tag => tag.Documents)
                .Map(
                    configuration => configuration.ToTable("DocumentTags").MapLeftKey("DocumentId").MapRightKey("TagId"));
            this.HasMany(document => document.Versions).WithRequired(version => version.Document);
        }
    }

    public class DocumentMapping<T> : SystemEntityMapping<T> where T : Document, new()
    {
        public DocumentMapping()
        {
            this.Map(configuration => configuration.Requires("DocumentType").HasValue(typeof (T).FullName));
        }

        public override Type BaseType
        {
            get { return typeof(Document); }
        }
    }
}