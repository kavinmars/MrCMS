using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using Ninject;

namespace MrCMS.Website.Binders
{
    public class AddMessageTemplateGetModelBinder : MessageTemplateModelBinder
    {
        public override object BindMrCMSModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);
            return model;
        }
    }

    public class AddMessageTemplateModelBinder : MessageTemplateModelBinder
    {
        public override object BindMrCMSModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var type = GetTypeByName(controllerContext);

            bindingContext.ModelMetadata =
                ModelMetadataProviders.Current.GetMetadataForType(
                    () => CreateModel(controllerContext, bindingContext, type), type);

            var messageTemplate = base.BindModel(controllerContext, bindingContext) as MessageTemplate;

            return messageTemplate;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var type = GetTypeByName(controllerContext);
            return Activator.CreateInstance(type);
        }

        private static Type GetTypeByName(ControllerContext controllerContext)
        {
            var valueFromContext = GetValueFromContext(controllerContext, "MessageTemplateType");
            return DocumentMetadataHelper.GetTypeByName(valueFromContext)
                ?? TypeHelper.MappedClasses.FirstOrDefault(x => x.Name == valueFromContext);
        }
    }
}