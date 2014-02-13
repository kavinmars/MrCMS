using System.Collections.Generic;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using System.Linq;

namespace MrCMS.Tests.Stubs
{
    public class StubWebpage : Webpage
    {
        public StubWebpage()
        {
            FormProperties = new MrCMSList<FormProperty>();
        }
        public virtual void SetChildren(IList<Webpage> children)
        {
            Children = children.OfType<Document>().ToMrCMSCollection();
            foreach (var document in Children)
            {
                document.Parent = this;
            }
        }
    }
}