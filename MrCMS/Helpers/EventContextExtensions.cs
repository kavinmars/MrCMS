using Microsoft.Owin;
using MrCMS.Services;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Helpers
{
    public static class EventContextExtensions
    {
        public static IEventContext EventContext(this IOwinContext context)
        {
            return context.Get<IEventContext>(KernelCreator.EventContextKey);
        }
    }
}