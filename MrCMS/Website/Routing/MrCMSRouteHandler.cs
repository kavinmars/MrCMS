using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class MrCMSAspxRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSAspxHttpHandler>();
                mrCMSHttpHandler.SetRequestContext(requestContext);
                return mrCMSHttpHandler;
            }
            else
            {
                return new NotInstalledHandler();
            }
        }
    }

    public class MrCMSAspxHttpHandler : ErrorHandlingHttpHandler
    {
        private readonly IDbContext _dbContext;
        private readonly SiteSettings _siteSettings;

        public MrCMSAspxHttpHandler(IDbContext dbContext, IDocumentService documentService, IControllerManager controllerManager, SiteSettings siteSettings)
            : base(documentService, controllerManager)
        {
            _dbContext = dbContext;
            _siteSettings = siteSettings;
        }

        public override void ProcessRequest(HttpContext context)
        {
            // Wrapped up to aid testing
            ProcessRequest(new HttpContextWrapper(context));
        }

        public override bool IsReusable
        {
            get { return false; }
        }

        private void ProcessRequest(HttpContextWrapper context)
        {
            var urlHistory =
                _dbContext.Set<UrlHistory>()
                          .FirstOrDefault(
                              history => history.UrlSegment == Data && history.Site.Id == _siteSettings.Site.Id);
            if (urlHistory != null && urlHistory.Webpage != null)
            {
                context.Response.RedirectPermanent("~/" + urlHistory.Webpage.LiveUrlSegment);
            }

            HandleError(context, 404, _siteSettings.Error404PageId, new HttpException(404, "Cannot find " + Data), _siteSettings.Log404s);
        }

        private string Data
        {
            get { return Convert.ToString(RequestContext.RouteData.Values["data"]); }
        }
    }

    public class MrCMSRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSHttpHandler>();
                mrCMSHttpHandler.SetRequestContext(requestContext);
                return mrCMSHttpHandler;
            }
            else
            {
                return new NotInstalledHandler();
            }
        }
    }

    public class NotInstalledHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("~/Install");
        }

        public bool IsReusable { get; private set; }
    }
}