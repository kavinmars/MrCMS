using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Binders
{
    public abstract class MessageTemplateModelBinder : MrCMSDefaultModelBinder
    {
        protected readonly IMessageTemplateService MessageTemplateService;

        protected MessageTemplateModelBinder(IDbContext dbContext, IMessageTemplateService messageTemplateService)
            : base(() => dbContext)
        {
            this.MessageTemplateService = messageTemplateService;
        }
    }
}