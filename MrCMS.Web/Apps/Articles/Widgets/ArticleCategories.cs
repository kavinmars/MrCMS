using System;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using Ninject;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleCategories : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        public override object GetModel(IKernel kernel)
        {
            if (ArticleList != null)
                return ArticleList;

            return kernel.Get<IDbContext>().Set<ArticleList>().FirstOrDefault();
        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, IKernel kernel)
        {
            viewData["ArticleLists"] = kernel.Get<IDbContext>().Set<ArticleList>()
                                       .OrderByDescending(list => list.Name)
                                       .ToList()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select an article list...");
        }

    }
}
