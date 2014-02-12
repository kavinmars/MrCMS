using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Binders
{
    public class EditDocumentModelBinder : DocumentModelBinder
    {
        public EditDocumentModelBinder(IDbContext dbContext, IDocumentService documentService) : base(dbContext, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var document = base.BindModel(controllerContext, bindingContext) as Document;

            var taglist = GetValueFromContext(controllerContext, "TagList");
            DocumentService.SetTags(taglist, document);

            if (document is Webpage)
            {
                var frontEndRoles = GetValueFromContext(controllerContext, "FrontEndRoles");
                DocumentService.SetFrontEndRoles(frontEndRoles, document as Webpage);
            }

            return document;
        }
    }
}