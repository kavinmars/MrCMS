using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage, IBelongToUser
    {
        [AllowHtml]
        [StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string Abstract { get; set; }

        [DisplayName("Author")]
        public virtual User User { get; set; }

        public override void AdminViewData(ViewDataDictionary viewData, IKernel kernel)
        {
            base.AdminViewData(viewData, kernel);
            viewData["users"] = kernel.Get<IDbContext>().Set<User>()
                                       .ToList()
                                       .BuildSelectItemList(user => user.Name, user => user.Id.ToString(),
                                                            user => user == User, "Please select ...");
        }
    }
}