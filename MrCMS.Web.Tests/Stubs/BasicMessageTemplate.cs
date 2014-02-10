using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Web.Tests.Stubs
{
    public class BasicMessageTemplate : MessageTemplate
    {
        public override MessageTemplate GetInitialTemplate(IDbContext dbContext)
        {
            return this;
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return new List<string>();
        }
    }
}