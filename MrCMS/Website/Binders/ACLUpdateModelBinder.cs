﻿using MrCMS.DataAccess;
using MrCMS.Helpers;
using MrCMS.Models;
using System.Linq;

namespace MrCMS.Website.Binders
{
    public class ACLUpdateModelBinder : MrCMSDefaultModelBinder
    {
        public ACLUpdateModelBinder(IDbContext dbContext)
            : base(() => dbContext)
        {
        }

        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var nameValueCollection = controllerContext.HttpContext.Request.Form;

            var keys = nameValueCollection.AllKeys.Where(s => s.StartsWith("acl-"));

            return keys.Select(s =>
                                   {
                                       var substring = s.Substring(4).Split('-');
                                       return new ACLUpdateRecord
                                                  {
                                                      Role = substring[0],
                                                      Key = substring[1],
                                                      Allowed = nameValueCollection[s].Contains("true")
                                                  };
                                   }).ToList();
        }
    }
}