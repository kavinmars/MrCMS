using System;
using System.Collections.Generic;
using MrCMS.DataAccess;
using MrCMS.DataAccess.Mappings;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    //public class TestMessageTemplate : MessageTemplate, IMessageTemplate<User>
    //{
    //    public override MessageTemplate GetInitialTemplate(IDbContext dbContext)
    //    {
    //        return new TestMessageTemplate()
    //        {
    //            FromAddress = "test@example.com",
    //            FromName = "test@example.com",
    //            ToAddress = "{Email}",
    //            ToName = "{Name}",
    //            Bcc = String.Empty,
    //            Cc = String.Empty,
    //            Subject = String.Format("{0} - Password Reset Request", CurrentRequestData.CurrentSite.Name),
    //            Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
    //            IsHtml = true
    //        };
    //    }

    //    public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
    //    {
    //        return messageTemplateParser.GetAllTokens<User>();
    //    }
    //}
    public class ResetPasswordMessageTemplate : MessageTemplate, IMessageTemplate<User>
    {
        public override MessageTemplate GetInitialTemplate(IDbContext dbContext)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            return new ResetPasswordMessageTemplate
            {
                FromAddress = "test@example.com",
                FromName = "test@example.com",
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Password Reset Request", fromName),
                Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
                IsHtml = true
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<User>();
        }
    }
}