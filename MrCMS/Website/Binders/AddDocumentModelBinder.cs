using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Binders
{
    public class AddDocumentGetModelBinder : DocumentModelBinder
    {
        public AddDocumentGetModelBinder(IDbContext dbContext, IDocumentService documentService)
            : base(dbContext, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
            return model;
        }
    }

    public class AddDocumentModelBinder : DocumentModelBinder
    {
        public AddDocumentModelBinder(IDbContext dbContext, IDocumentService documentService)
            : base(dbContext, documentService)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var document = base.BindModel(controllerContext, bindingContext) as Document;

            // Extension method used to break cache
            document.SetParent(document.Parent);
            //set include as navigation as default
            if (document is Webpage)
            {
                (document as Webpage).RevealInNavigation = true;

                var pages = (document.Parent == null
                                 ? Session.Set<Webpage>().Where(webpage => webpage.Parent == null).ToList()
                                 : document.Parent.Children.OfType<Webpage>()).ToList();
                document.DisplayOrder = pages.Any() ? pages.Max(x => x.DisplayOrder) + 1 : 0;
            }

            return document;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var type = GetTypeByName(controllerContext);
            return Activator.CreateInstance(type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            string valueFromContext = GetValueFromContext(controllerContext, "DocumentType");
            return DocumentMetadataHelper.GetTypeByName(valueFromContext)
                ?? TypeHelper.MappedClasses.FirstOrDefault(x => x.Name == valueFromContext);
        }
    }
}