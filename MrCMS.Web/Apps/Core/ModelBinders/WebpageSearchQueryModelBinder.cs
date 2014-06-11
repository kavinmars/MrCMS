using System.Web.Mvc;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class WebpageSearchQueryModelBinder : MrCMSDefaultModelBinder
    {
        public override object BindMrCMSModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int page;
            int.TryParse(GetValueFromContext(controllerContext, "page"), out page);
            if (page == 0)
                page = 1;
            return new WebpageSearchQuery
            {
                Page = page,
                Term = GetValueFromContext(controllerContext, "term")
            };
        }
    }
}