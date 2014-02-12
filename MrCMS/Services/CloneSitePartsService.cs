using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class CloneSitePartsService : ICloneSitePartsService
    {
        private readonly IDbContext _dataContext;

        public CloneSitePartsService(IDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void CopySettings(Site @from, Site to)
        {
            _dataContext.Transact(session =>
            {
                var fromProvider = new ConfigurationProvider(new SettingService(session, @from), @from);
                var toProvider = new ConfigurationProvider(new SettingService(session, @to), @to);
                var siteSettingsBases = fromProvider.GetAllSiteSettings();
                siteSettingsBases.ForEach(@base =>
                {
                    @base.Site = to;
                    toProvider.SaveSettings(@base);
                });
            });
        }

        public void CopyLayouts(Site @from, Site to)
        {
            var layouts =
                _dataContext.Query<Layout>().Where(layout => layout.Site == @from && layout.Parent == null).ToList();

            var copies = layouts.Select(layout => CopyLayout(layout, to));

            _dataContext.Transact(session => copies.ForEach(layout =>
            {
                session.Add(layout);
                layout.LayoutAreas.ForEach(area =>
                {
                    session.Add(area);
                    area.Widgets.ForEach(widget => session.Add(widget));
                });
            }));
        }

        private Layout CopyLayout(Layout layout, Site to)
        {
            var copy = GetCopy(layout, to);
            copy.LayoutAreas = layout.LayoutAreas.Select(area =>
            {
                var areaCopy = GetCopy(area, to);
                areaCopy.Layout = copy;
                areaCopy.Widgets = area.Widgets
                                       .Where(widget => widget.Webpage == null)
                                       .Select(widget =>
                                       {
                                           var widgetCopy = GetCopy(widget, to);
                                           widgetCopy.LayoutArea = areaCopy;
                                           return widgetCopy;
                                       })
                                       .ToList();
                return areaCopy;
            }).ToList();
            copy.Children = layout.Children.OfType<Layout>().Select(childLayout =>
            {
                var child = CopyLayout(childLayout, to);
                child.Parent = copy;
                return child;
            }).Cast<Document>().ToList();
            return copy;
        }

        private T GetCopy<T>(T entity, Site site) where T : SiteEntity
        {
            var shallowCopy = entity.ShallowCopy();
            shallowCopy.Site = site;
            return shallowCopy;
        }

        public void CopyMediaCategories(Site @from, Site to)
        {
            var mediaCategories = _dataContext.Query<MediaCategory>().Where(category => category.Site == @from && category.Parent == null).ToList();

            var copies = mediaCategories.Select(category => CopyMediaCategory(category, to));

            _dataContext.Transact(session => copies.ForEach(category => session.Add(category)));
        }

        private MediaCategory CopyMediaCategory(MediaCategory category, Site to)
        {
            var copy = GetCopy(category, to);
            copy.Children =
                category.Children.OfType<MediaCategory>()
                        .Select(childLayout =>
                        {
                            var child = CopyMediaCategory(childLayout, to);
                            child.Parent = copy;
                            return child;
                        })
                        .Cast<Document>()
                        .ToList();
            return copy;
        }

        public void CopyHome(Site @from, Site to)
        {
            var home =
                _dataContext.Query<Webpage>()
                            .OrderBy(webpage => webpage.DisplayOrder)
                            .FirstOrDefault(webpage => webpage.Site == @from && webpage.Parent == null);

            var copy = GetCopy(home, to);
            _dataContext.Transact(session => session.Add(copy));
        }

        public void Copy404(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(new SettingService(_dataContext, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_dataContext, @to), @to);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error404 = _dataContext.Get<Webpage>(siteSettings.Error404PageId);

            var copy = GetCopy(error404, to);
            _dataContext.Transact(session => session.Add(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error404PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void Copy403(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(new SettingService(_dataContext, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_dataContext, @to), @to);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error403 = _dataContext.Get<Webpage>(siteSettings.Error403PageId);

            var copy = GetCopy(error403, to);
            _dataContext.Transact(session => session.Add(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error403PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void Copy500(Site @from, Site to)
        {
            var fromProvider = new ConfigurationProvider(new SettingService(_dataContext, @from), @from);
            var toProvider = new ConfigurationProvider(new SettingService(_dataContext, @to), @to);
            var siteSettings = fromProvider.GetSiteSettings<SiteSettings>();
            var error500 = _dataContext.Get<Webpage>(siteSettings.Error500PageId);

            var copy = GetCopy(error500, to);
            _dataContext.Transact(session => session.Add(copy));

            var toSettings = toProvider.GetSiteSettings<SiteSettings>();
            toSettings.Error500PageId = copy.Id;
            toProvider.SaveSettings(toSettings);
        }

        public void CopyLogin(Site @from, Site to)
        {
            var login =
                _dataContext.Query<Webpage>()
                            .OrderBy(webpage => webpage.DisplayOrder)
                            .FirstOrDefault(
                                webpage =>
                                webpage.Site == @from && webpage.ObjectTypeName == "MrCMS.Web.Apps.Core.Pages.LoginPage");

            if (login != null)
            {
                var loginCopy = GetCopy(login, to);
                _dataContext.Transact(session => session.Add(loginCopy));
            }
        }
    }
}