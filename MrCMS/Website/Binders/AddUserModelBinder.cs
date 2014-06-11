using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Binders
{
    public class AddUserModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IPasswordManagementService _passwordManagementService;

        public AddUserModelBinder(IPasswordManagementService passwordManagementService) 
        {
            _passwordManagementService = passwordManagementService;
        }

        public override object BindMrCMSModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = base.BindModel(controllerContext, bindingContext) as User;

            _passwordManagementService.SetPassword(user,
                                                   controllerContext.GetValueFromRequest("Password"),
                                                   controllerContext.GetValueFromRequest("ConfirmPassword"));

            return user;
        }
    }
}