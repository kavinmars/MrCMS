using System;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Entities.Widget;

namespace MrCMS.DataAccess.Mappings
{
    public class WidgetMapping<T> : SystemEntityMapping<T> where T : Widget, new()
    {
        public WidgetMapping()
        {
            this.Map(configuration => configuration.Requires("WidgetType").HasValue(typeof(T).FullName));
        }

        public override Type BaseType
        {
            get { return typeof(Widget); }
        }
    }
    public class FormPropertyMapping<T> : SystemEntityMapping<T> where T : FormProperty, new()
    {
        public FormPropertyMapping()
        {
            this.Map(configuration => configuration.Requires("PropertyType").HasValue(typeof(T).FullName));
        }

        public override Type BaseType
        {
            get { return typeof(FormProperty); }
        }
    }
}