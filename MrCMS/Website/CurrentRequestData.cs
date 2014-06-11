using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Elmah;
using Microsoft.Owin;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using NHibernate;
using Ninject;
using Owin;

namespace MrCMS.Website
{
    public class CurrentRequestData
    {
        private const string UserSessionId = "current.usersessionGuid";
        private static bool? _databaseIsInstalled;

        public static ErrorSignal ErrorSignal
        {
            get { return (ErrorSignal)CurrentContext.Get<ErrorSignal>("current.errorsignal"); }
            set { CurrentContext.Set("current.errorsignal", value); }
        }

        public static User CurrentUser
        {
            get { return CurrentContext.Get<User>("current.user"); }
            set { CurrentContext.Set("current.user", value); }
        }

        public static Site CurrentSite
        {
            get
            {
                return CurrentContext.Get<Site>("current.site") ??
                       (CurrentSite = CurrentContext.GetKernel().Get<ICurrentSiteLocator>().GetCurrentSite());
            }
            set
            {
                CurrentContext.Set("current.site", value);
                SetSiteFilter(value);
            }
        }

        public static Webpage CurrentPage
        {
            get { return CurrentContext.Get<Webpage>("current.webpage"); }
            set { CurrentContext.Set("current.webpage", value); }
        }

        public static SiteSettings SiteSettings
        {
            get { return CurrentContext.Get<SiteSettings>("current.sitesettings"); }
            set { CurrentContext.Set("current.sitesettings", value); }
        }

        public static Webpage HomePage
        {
            get { return CurrentContext.Get<Webpage>("current.homepage"); }
            set { CurrentContext.Set("current.homepage", value); }
        }

        public static CultureInfo CultureInfo
        {
            get
            {
                if (SiteSettings != null)
                {
                    if (CurrentContext.Get<CultureInfo>("current.cultureinfo") == null)
                        CurrentContext.Set("current.cultureinfo", SiteSettings.CultureInfo);
                    return CurrentContext.Get<CultureInfo>("current.cultureinfo");
                }
                return CultureInfo.CurrentCulture;
            }
        }

        public static TimeZoneInfo TimeZoneInfo
        {
            get
            {
                //return TimeZoneInfo.Local;
                return SiteSettings != null
                    ? (SiteSettings.TimeZoneInfo ?? TimeZoneInfo.Local)
                    : TimeZoneInfo.Local;
            }
        }

        public static DateTime Now
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo); }
        }

        public static IOwinContext CurrentContext
        {
            get { return new OwinContext(OwinRequestScopeContext.Current.Environment); }
        }

        public static bool CurrentUserIsAdmin
        {
            get { return CurrentUser != null && CurrentUser.IsAdmin; }
        }

        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    string applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

                    string connectionStrings = Path.Combine(applicationPhysicalPath, "ConnectionStrings.config");

                    if (!File.Exists(connectionStrings))
                    {
                        File.WriteAllText(connectionStrings, "<connectionStrings></connectionStrings>");
                    }

                    ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["mrcms"];
                    _databaseIsInstalled = connectionString != null &&
                                           !String.IsNullOrEmpty(connectionString.ConnectionString);
                }
                return _databaseIsInstalled.Value;
            }
            set { _databaseIsInstalled = value; }
        }

        private static Guid? UserGuidOverride
        {
            get { return CurrentContext.Get<Guid?>(UserSessionId); }
            set { CurrentContext.Set(UserSessionId, value); }
        }

        public static Guid UserGuid
        {
            get
            {
                if (UserGuidOverride.HasValue)
                    return UserGuidOverride.Value;
                if (CurrentUser != null) return CurrentUser.Guid;
                string o = CurrentContext.Request.Cookies[UserSessionId];
                Guid result;
                if (o == null || !Guid.TryParse(o, out result))
                {
                    result = Guid.NewGuid();
                    AddCookieToResponse(UserSessionId, result.ToString(), Now.AddMonths(3));
                }
                return result;
            }
            set { UserGuidOverride = value; }
        }

        public static HashSet<Action<IKernel>> OnEndRequest
        {
            get
            {
                if (CurrentContext.Get<HashSet<Action<IKernel>>>("current.on-end-request") == null)
                    CurrentContext.Set("current.on-end-request", new HashSet<Action<IKernel>>());
                return CurrentContext.Get<HashSet<Action<IKernel>>>("current.on-end-request");
            }
            set { CurrentContext.Set("current.on-end-request", value); }
        }

        public static HashSet<QueuedTask> QueuedTasks
        {
            get
            {
                if (CurrentContext.Get<HashSet<QueuedTask>>("current.queued-tasks") == null)
                    CurrentContext.Set("current.queued-tasks", new HashSet<QueuedTask>());
                return CurrentContext.Get<HashSet<QueuedTask>>("current.queued-tasks");
            }
            set { CurrentContext.Set("current.queued-tasks", value); }
        }

        private static void SetSiteFilter(Site value)
        {
            var session = CurrentContext.GetKernel().Get<ISession>();
            if (value != null)
            {
                session.EnableFilter("SiteFilter").SetParameter("site", value.Id);
            }
            else
            {
                IFilter enabledFilter = session.GetEnabledFilter("SiteFilter");
                if (enabledFilter != null)
                {
                    session.DisableFilter("SiteFilter");
                }
            }
        }

        private static void AddCookieToResponse(string key, string value, DateTime expiry)
        {
            CurrentContext.Response.Cookies.Append(key, value, new CookieOptions {Expires = expiry});
        }
    }
}