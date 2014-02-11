using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web.WebPages.Scope;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Logging;

namespace MrCMS.DataAccess.Mappings
{
    public interface IMapDbModel
    {
        void Map(DbModelBuilder dbModelBuilder);
    }


    public abstract class SystemEntityMapping<T> : EntityTypeConfiguration<T>, IMapDbModel where T : SystemEntity
    {
        public void Map(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Configurations.Add(this);
        }
    }

    public static class DbMappings
    {
        private static Dictionary<Type, IMapDbModel> _mappers;
        public static Dictionary<Type, IMapDbModel> Mappers
        {
            get { return _mappers = _mappers ?? GetMappers(); }
        }

        private static Dictionary<Type, IMapDbModel> GetMappers()
        {
            var dictionary = new Dictionary<Type, IMapDbModel>();

            foreach (var type in TypeHelper.GetMappedClassesAssignableFrom<SystemEntity>().Where(type => !type.ContainsGenericParameters))
            {
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(SystemEntityMapping<>).MakeGenericType(type));
                if (types.Any())
                {
                    var mapDbModel = Activator.CreateInstance(types.First()) as IMapDbModel;
                    dictionary.Add(type, mapDbModel);
                }
                else
                {
                    var mapDbModel =
                        Activator.CreateInstance(typeof(DefaultSystemEntityMapping<>).MakeGenericType(type)) as
                        IMapDbModel;
                    dictionary.Add(type, mapDbModel);
                }
            }
            return dictionary;
        }
    }

    public class DefaultSystemEntityMapping<T> : SystemEntityMapping<T> where T : SystemEntity
    {
    }

    public class LogMapping : SystemEntityMapping<Log>
    {
        public LogMapping()
        {
            Ignore(log => log.Error);
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
            //Property(webpage => webpage.MetaTitle).HasColumnName("MetaTitle");
        }
    }

    public class MediaCategoryMapping : SystemEntityMapping<MediaCategory>
    {
        public MediaCategoryMapping()
        {
            //Property(mediaCategory => mediaCategory.MetaTitle).HasColumnName("MetaTitle");
        }
    }
    //public class WidgetMapping : SystemEntityMapping<Widget>
    //{
    //    public WidgetMapping()
    //    {
    //    }
    //    public override void Map(DbModelBuilder dbModelBuilder)
    //    {
    //        dbModelBuilder.Configurations.Add(this);
    //    }
    //}
}