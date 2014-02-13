using MrCMS.DataAccess.CustomCollections;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        public Tag()
        {
            Documents = new MrCMSSet<Document>();
        }
        public virtual string Name { get; set; }

        public virtual MrCMSSet<Document> Documents { get; set; }
    }
}