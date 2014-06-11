using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{
    public class ThirdPartyAuthSettingsModelBinder : MrCMSDefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext,
            Type modelType)
        {
            return controllerContext.HttpContext.Get<ThirdPartyAuthSettings>();
        }
    }
}