using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using NHibernate;
using Ninject;
using FormCollection = System.Web.Mvc.FormCollection;

namespace MrCMS.Website.Routing
{
    public interface IControllerManager
    {
        void SetViewData(Webpage webpage, Controller controller, ISession session);
        void SetFormData(Webpage webpage, Controller controller, NameValueCollection form);
        Controller GetController(RequestContext requestContext, Webpage webpage, string httpMethod);
    }

    public class ControllerManager : IControllerManager
    {
        public void SetViewData(Webpage webpage, Controller controller, ISession session)
        {
            if (controller.Request.HttpMethod == "GET" && webpage != null)
            {
                webpage.UiViewData(controller.ViewData, session, controller.Request);
            }
        }

        public Func<Document, DocumentMetadata> GetMetadata = document => document.GetMetadata();

        public void SetFormData(Webpage webpage, Controller controller, NameValueCollection form)
        {
            if (form != null)
            {
                var formCollection = new FormCollection(form);
                var metadata = GetMetadata(webpage);
                if (metadata != null && metadata.PostTypes != null && metadata.PostTypes.Any())
                {
                    foreach (var type in metadata.PostTypes)
                    {
                        var modelBinder = ModelBinders.Binders.GetBinder(type) as MrCMSDefaultModelBinder;
                        if (modelBinder != null)
                        {
                            var modelBindingContext = new ModelBindingContext
                                                          {
                                                              ValueProvider = formCollection,
                                                              ModelMetadata =
                                                                  ModelMetadataProviders.Current.GetMetadataForType(
                                                                      () =>
                                                                      modelBinder.GetModelFromSession(
                                                                          controller.ControllerContext,
                                                                          string.Empty, type), type)
                                                          };

                            var model = modelBinder.BindModel(controller.ControllerContext, modelBindingContext);
                            controller.RouteData.Values[type.Name.ToLower()] = model;
                        }
                    }
                }
                else
                {
                    controller.RouteData.Values["form"] = formCollection;
                }
            }
        }

        public string GetActionName(Webpage webpage, string httpMethod)
        {
            if (webpage == null)
                return null;

            if (!webpage.Published && !webpage.IsAllowed(CurrentRequestData.CurrentUser))
                return null;

            var metadata = GetMetadata(webpage);

            if (metadata == null) return null;

            switch (httpMethod)
            {
                case "GET":
                case "HEAD":
                    return metadata.WebGetAction;
                case "POST":
                    return metadata.WebPostAction;
                default:
                    return null;
            }
        }

        public Controller GetController(RequestContext requestContext, Webpage webpage, string httpMethod)
        {
            var controllerName = GetControllerName(webpage, httpMethod);

            var controller =
                requestContext.GetKernel()
                    .Get(MrCMSControllerFactory.FindControllerType(requestContext, controllerName)) as Controller;

            controller.ControllerContext = new ControllerContext(requestContext, controller);

            var routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary["controller"] = controllerName;
            routeValueDictionary["action"] = GetActionName(webpage, httpMethod);
            routeValueDictionary["page"] = webpage;
            controller.RouteData.Values.Merge(routeValueDictionary);
            controller.RouteData.DataTokens["app"] = MrCMSApp.AppWebpages[webpage.GetType()];

            return controller;
        }


        public string GetControllerName(Webpage webpage, string httpMethod)
        {
            if (webpage == null)
                return null;

            if (!webpage.Published && !webpage.IsAllowed(CurrentRequestData.CurrentUser))
                return null;

            var metadata = GetMetadata(webpage);

            if (metadata == null) return null;

            string controllerName;

            switch (httpMethod)
            {
                case "GET":
                case "HEAD":
                    controllerName = metadata.WebGetController;
                    break;
                case "POST":
                    controllerName = metadata.WebPostController;
                    break;
                default:
                    return null;
            }

            return controllerName;
        }
    }
}