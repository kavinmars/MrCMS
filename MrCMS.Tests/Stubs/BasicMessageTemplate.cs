using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Tests.Stubs
{
    //[MrCMSMapClass]
    public class BasicMessageTemplate : MessageTemplate
    {
        public override MessageTemplate GetInitialTemplate(IDbContext dbContext)
        {
            return new BasicMessageTemplate()
            {
                ToAddress = "{Email}",
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return new List<string>();
        }
    }
}