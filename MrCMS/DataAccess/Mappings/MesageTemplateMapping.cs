using System;
using MrCMS.Entities.Messaging;

namespace MrCMS.DataAccess.Mappings
{
    public class MesageTemplateMapping<T> : SystemEntityMapping<T> where T : MessageTemplate, new()
    {
        public MesageTemplateMapping()
        {
            this.Map(configuration => configuration.Requires("MessageTemplateType").HasValue(typeof(T).FullName));
        }

        public override Type BaseType
        {
            get { return typeof(MessageTemplate); }
        }
    }
}