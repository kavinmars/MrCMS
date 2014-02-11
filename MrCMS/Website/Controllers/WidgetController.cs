using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Helpers;

namespace MrCMS.Website.Controllers
{
    public class WidgetController : MrCMSUIController
    {
        private readonly IWidgetService _widgetService;

        public WidgetController(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public PartialViewResult Show(Widget widget)
        {
            if (MrCMSApp.AppWidgets.ContainsKey(widget.ObjectType))
                RouteData.DataTokens["app"] = MrCMSApp.AppWidgets[widget.ObjectType];
            return PartialView(
                !string.IsNullOrWhiteSpace(widget.CustomLayout) ? widget.CustomLayout : widget.ObjectTypeName,
                _widgetService.GetModel(widget));
        }
    }
}