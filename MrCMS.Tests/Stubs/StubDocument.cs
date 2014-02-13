using System.Collections.Generic;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Stubs
{
    //[MrCMSMapClass]
    public class StubDocument : Document
    {
        public virtual void SetChildren(IList<Document> children)
        {
            Children = children.ToMrCMSCollection();
            foreach (var document in Children)
            {
                document.Parent = this;
            }
        }

        public virtual void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions.ToMrCMSList();
        }
    }
    //[MrCMSMapClass]
    public class StubUniquePage : IUniquePage
    {
    }
}