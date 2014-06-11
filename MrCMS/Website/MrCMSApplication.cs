using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MrCMS.Apps;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Filters;
using MrCMS.Website.Routing;
using NHibernate;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;

namespace MrCMS.Website
{
    public abstract class MrCMSApplication : HttpApplication
    {
        public const string AssemblyVersion = "0.4.1.0";
        public const string AssemblyFileVersion = "0.4.1.0";

        protected static IEnumerable<string> WebExtensions
        {
            get
            {
                yield return ".aspx";
                yield return ".php";
            }
        }

        public abstract string RootNamespace { get; }

        public static bool InDevelopment
        {
            get
            {
                return "true".Equals(ConfigurationManager.AppSettings["Development"],
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        protected void Application_Start()
        {
            MrCMSApp.RegisterAllApps();
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            RegisterServices(KernelCreator.Kernel);
            MrCMSApp.RegisterAllServices(KernelCreator.Kernel);


            ModelBinders.Binders.DefaultBinder = new MrCMSDefaultModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), new CultureAwareDateBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableCultureAwareDateBinder());


            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Insert(0, new MrCMSRazorViewEngine());

            ControllerBuilder.Current.SetControllerFactory(new MrCMSControllerFactory());

            GlobalFilters.Filters.Add(new HoneypotFilterAttribute());

            ModelMetadataProviders.Current = new MrCMSMetadataProvider(KernelCreator.Kernel);
        }


        private static bool IsFileRequest(Uri uri)
        {
            string absolutePath = uri.AbsolutePath;
            if (string.IsNullOrWhiteSpace(absolutePath))
                return false;
            string extension = Path.GetExtension(absolutePath);

            return !string.IsNullOrWhiteSpace(extension) && !WebExtensions.Contains(extension);
        }

        public override void Init()
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                //BeginRequest += (sender, args) =>
                //{

                //    if (!IsFileRequest(Request.Url))
                //    {
                //    }
                //};
                AuthenticateRequest += (sender, args) =>
                {
                    if (!IsFileRequest(Request.Url))
                    {
                        if (CurrentRequestData.CurrentContext.User != null)
                        {
                            IKernel kernel = Context.GetKernel();
                            User currentUser = kernel.Get<IUserService>().GetCurrentUser(CurrentRequestData.CurrentContext);
                            if (!Request.Url.AbsolutePath.StartsWith("/signalr/") && currentUser == null ||
                                !currentUser.IsActive)
                                kernel.Get<IAuthorisationService>().Logout();
                            else
                                CurrentRequestData.CurrentUser = currentUser;
                        }
                    }
                };
                EndRequest += (sender, args) =>
                {
                    IKernel kernel = Context.GetKernel();
                    if (CurrentRequestData.QueuedTasks.Any())
                    {
                        kernel.Get<ISession>()
                            .Transact(session =>
                            {
                                foreach (QueuedTask queuedTask in CurrentRequestData.QueuedTasks)
                                    session.Save(queuedTask);
                            });
                    }
                    foreach (var action in CurrentRequestData.OnEndRequest)
                        action(kernel);
                };
            }
            else
            {
                EndRequest += (sender, args) =>
                {
                    foreach (var action in CurrentRequestData.OnEndRequest)
                        action(Context.GetKernel());
                };
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("InstallerRoute", "install", new { controller = "Install", action = "Setup" });
            routes.MapRoute("Task Execution", "execute-pending-tasks",
                new { controller = "TaskExecution", action = "Execute" });
            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "SEO", action = "Sitemap" });
            routes.MapRoute("robots.txt", "robots.txt", new { controller = "SEO", action = "Robots" });
            routes.MapRoute("ckeditor Config", "Areas/Admin/Content/Editors/ckeditor/config.js",
                new { controller = "CKEditor", action = "Config" });

            routes.MapRoute("Logout", "logout", new { controller = "Login", action = "Logout" },
                new[] { RootNamespace });

            routes.MapRoute("zones", "render-widget", new { controller = "Widget", action = "Show" },
                new[] { RootNamespace });

            routes.MapRoute("ajax content save", "admintools/savebodycontent",
                new { controller = "AdminTools", action = "SaveBodyContent" });

            routes.MapRoute("form save", "save-form/{id}", new { controller = "Form", action = "Save" });

            routes.Add(new Route("{*data}", new RouteValueDictionary(),
                new RouteValueDictionary(new { data = @".*\.aspx" }),
                new MrCMSAspxRouteHandler()));
            routes.Add(new Route("{*data}", new RouteValueDictionary(), new RouteValueDictionary(),
                new MrCMSRouteHandler()));
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        protected abstract void RegisterServices(IKernel kernel);

        //public static IEnumerable<T> GetAll<T>()
        //{
        //    return Kernel.GetAll<T>();
        //}

        //public static void OverrideKernel(IKernel kernel)
        //{
        //    _kernel = kernel;
        //}

        //public static T Get<T>()
        //{
        //    return Kernel.Get<T>();
        //}

        //public static object Get(Type type)
        //{
        //    return Kernel.Get(type);
        //}
    }
}