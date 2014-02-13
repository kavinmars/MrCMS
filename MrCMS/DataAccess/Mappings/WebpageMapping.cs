using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.DataAccess.Mappings
{
    public class UserMapping : SystemEntityMapping<User>
    {
        public UserMapping()
        {
            HasMany(user => user.UserProfileData).WithRequired(data => data.User);
        }
    }
    public class WebpageMapping : SystemEntityMapping<Webpage>
    {
        public WebpageMapping()
        {
            HasMany(webpage => webpage.Widgets).WithOptional(widget => widget.Webpage);
            HasMany(webpage => webpage.ShownWidgets)
                .WithMany(widget => widget.ShownOn)
                .Map(configuration =>
                     {
                         configuration.ToTable("ShownWidgets");
                         configuration.MapLeftKey("WebpageId");
                         configuration.MapRightKey("WidgetId");
                     });
            HasMany(webpage => webpage.HiddenWidgets)
                .WithMany(widget => widget.HiddenOn)
                .Map(configuration =>
                     {
                         configuration.ToTable("HiddenWidgets");
                         configuration.MapLeftKey("WebpageId");
                         configuration.MapRightKey("WidgetId");
                     });
        }
    }
}