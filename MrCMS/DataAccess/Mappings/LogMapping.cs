using MrCMS.Entities.Messaging;
using MrCMS.Logging;

namespace MrCMS.DataAccess.Mappings
{
    public class LogMapping : SystemEntityMapping<Log>
    {
        public LogMapping()
        {
            Ignore(log => log.Error);
        }
    }
}