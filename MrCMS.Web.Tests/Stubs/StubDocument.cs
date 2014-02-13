using System.Collections.Generic;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Tests.Stubs
{
    public class StubDocument : Document
    {
        public void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions.ToMrCMSList();
        }
    }
    public class StubWebpage : Webpage	
    {
        public void SetVersions(List<DocumentVersion> versions)
        {
            Versions = versions.ToMrCMSList();
        }
    }
}