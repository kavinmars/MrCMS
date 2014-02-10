using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Binders
{
    public abstract class DocumentModelBinder : MrCMSDefaultModelBinder
    {
        protected readonly IDocumentService DocumentService;

        protected DocumentModelBinder(IDbContext dbContext, IDocumentService documentService)
            : base(() => dbContext)
        {
            this.DocumentService = documentService;
        }
    }
}