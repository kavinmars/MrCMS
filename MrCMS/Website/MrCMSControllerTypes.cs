using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Website
{
    public static class MrCMSControllerTypes
    {
        public static readonly Dictionary<string, List<Type>> AppUiControllers;
        public static readonly Dictionary<string, List<Type>> AppAdminControllers;
        public static readonly List<Type> UiControllers;
        public static readonly List<Type> AdminControllers;

        static MrCMSControllerTypes()
        {
            AppUiControllers =
                MrCMSApp.AppTypes.Where(pair => typeof (MrCMSUIController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(pair => pair.Key).ToList());
            AppAdminControllers =
                MrCMSApp.AppTypes.Where(pair => typeof (MrCMSAdminController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(pair => pair.Key).ToList());
            UiControllers =
                TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSUIController>()
                    .FindAll(type => !AppUiControllers.SelectMany(pair => pair.Value).Contains(type));
            AdminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSAdminController>()
                .FindAll(type => !AppAdminControllers.SelectMany(pair => pair.Value).Contains(type));
        }

        public static IEnumerable<Type> AllControllers
        {
            get
            {
                foreach (Type controller in UiControllers)
                    yield return controller;
                foreach (Type controller in AdminControllers)
                    yield return controller;
                foreach (Type controller in AppUiControllers.Keys.SelectMany(key => AppUiControllers[key]))
                    yield return controller;
                foreach (Type controller in AppAdminControllers.Keys.SelectMany(key => AppAdminControllers[key]))
                    yield return controller;
            }
        }

        public static IEnumerable<MethodInfo> GetActionMethods(Type controllerType)
        {
            return controllerType.GetMethods()
                .Where(
                    q =>
                        q.IsPublic &&
                        (typeof (ActionResult).IsAssignableFrom(q.ReturnType) ||
                         (q.ReturnType.IsGenericType && q.ReturnType.GetGenericTypeDefinition() == typeof (Task<>) &&
                          q.ReturnType.GetGenericArguments().Count() == 1 &&
                          typeof (ActionResult).IsAssignableFrom(q.ReturnType.GetGenericArguments()[0]))));
        }

        public static List<ActionMethodInfo<T>> GetActionMethodsWithAttribute<T>() where T : Attribute
        {
            IEnumerable<ReflectedControllerDescriptor> reflectedControllerDescriptors =
                AllControllers.Select(type => new ReflectedControllerDescriptor(type));
            IEnumerable<ActionDescriptor> descriptors = reflectedControllerDescriptors.SelectMany(
                controllerDescriptor =>
                    controllerDescriptor.GetCanonicalActions()
                        .Where(actionDescriptor => actionDescriptor.GetCustomAttributes(typeof (T), true).Any()));
            return descriptors.Select(descriptor => new ActionMethodInfo<T>
            {
                Descriptor = descriptor,
                Attribute =
                    descriptor.GetCustomAttributes(typeof (T), true)
                        .FirstOrDefault() as T
            }).ToList();
        }
    }
}