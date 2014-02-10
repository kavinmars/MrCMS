using System;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Website.Binders
{
    public class NullableEntityModelBinder : MrCMSDefaultModelBinder
    {
        public NullableEntityModelBinder(IDbContext dbContext)
            : base(() => dbContext)
        {
        }

        protected override bool ShouldReturnNull(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            const string baseId = "Id";
            var id = Convert.ToString(controllerContext.RouteData.Values[baseId] ??
                                      controllerContext.HttpContext.Request[baseId]);

            int intId;
            if (!int.TryParse(id, out intId) || intId <=0)
            {
                return true;
            }
            return false;
        }
    }
}