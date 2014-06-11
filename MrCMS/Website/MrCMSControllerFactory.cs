using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Properties;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Website.Controllers;
using Ninject;
using Ninject.Infrastructure.Language;

namespace MrCMS.Website
{
    public class MrCMSControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            return FindControllerType(requestContext, controllerName);
        }

        public static Type FindControllerType(RequestContext requestContext, string controllerName)
        {
            string appName = Convert.ToString(requestContext.RouteData.DataTokens["app"]);
            string areaName = Convert.ToString(requestContext.RouteData.DataTokens["area"]);

            var listToCheck = "admin".Equals(areaName, StringComparison.OrdinalIgnoreCase)
                ? GetAdminControllersToCheck(appName)
                : GetUiControllersToCheck(appName);

            Type controllerType =
                listToCheck.FirstOrDefault(
                    type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            return controllerType;
        }

        private static List<Type> GetAdminControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && MrCMSControllerTypes.AppAdminControllers.ContainsKey(appName))
                types.AddRange(MrCMSControllerTypes.AppAdminControllers[appName]);
            types.AddRange(MrCMSControllerTypes.AdminControllers);
            foreach (var key in
                    MrCMSControllerTypes.AppAdminControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(MrCMSControllerTypes.AppAdminControllers[key]);
            }
            return types;
        }

        private static List<Type> GetUiControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && MrCMSControllerTypes.AppUiControllers.ContainsKey(appName))
                types.AddRange(MrCMSControllerTypes.AppUiControllers[appName]);
            types.AddRange(MrCMSControllerTypes.UiControllers);
            foreach (
                var key in
                    MrCMSControllerTypes.AppUiControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(MrCMSControllerTypes.AppUiControllers[key]);
            }
            return types;
        }

        public bool IsValidControllerType(string appName, string controllerName, bool isAdmin)
        {
            string typeName = controllerName + "Controller";
            if (!String.IsNullOrWhiteSpace(appName))
            {
                return isAdmin
                    ? MrCMSControllerTypes.AppAdminControllers.ContainsKey(appName) && MrCMSControllerTypes.AppAdminControllers[appName].Any(
                        type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    : MrCMSControllerTypes.AppUiControllers.ContainsKey(appName) && MrCMSControllerTypes.AppUiControllers[appName].Any(
                        type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            }
            return isAdmin
                ? MrCMSControllerTypes.AdminControllers.Any(
                    type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                : MrCMSControllerTypes.UiControllers.Any(
                    type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        }
    }


    public class ActionMethodInfo<T>
    {
        public ActionDescriptor Descriptor { get; set; }
        public T Attribute { get; set; }
    }
}