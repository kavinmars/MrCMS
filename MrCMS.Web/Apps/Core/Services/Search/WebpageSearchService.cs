using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Indexing;
using MrCMS.Web.Apps.Core.Models.Search;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly ISearcher<Webpage, WebpageSearchIndexDefinition> _documentSearcher;
        private readonly IDocumentService _documentService;
        private readonly IOwinContext _owinContext;

        public WebpageSearchService(ISearcher<Webpage, WebpageSearchIndexDefinition> documentSearcher,
            IDocumentService documentService, IOwinContext owinContext)
        {
            _documentSearcher = documentSearcher;
            _documentService = documentService;
            _owinContext = owinContext;
        }

        public IPagedList<Webpage> Search(WebpageSearchQuery model)
        {
            return _documentSearcher.Search(model.GetQuery(_owinContext), model.Page);
        }

        public IEnumerable<Document> GetBreadCrumb(int? parentId)
        {
            return _documentService.GetParents(parentId).Reverse();
        }
    }
}