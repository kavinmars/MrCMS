using System.Collections.Generic;
using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class LatestXArticles : Widget
    {
        public virtual int NumberOfArticles { get; set; }
        public virtual ArticleList RelatedNewsList { get; set; }

        public override object GetModel(IKernel kernel)
        {
            if (RelatedNewsList == null)
                return null;


            return new LatestXArticlesViewModel
                       {
                           Articles = kernel.Get<IDbContext>().Query<Article>()
                                           .Where(article => article.Parent.Id == RelatedNewsList.Id && article.PublishOn != null && article.PublishOn <= CurrentRequestData.Now)
                                           .Take(NumberOfArticles)
                                           .ToList(),
                           Title = this.Name
                       };

        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, IKernel kernel)
        {
            viewData["newsList"] = kernel.Get<IDbContext>().Query<ArticleList>()
                                                .Where(article => article.PublishOn != null && article.PublishOn <= CurrentRequestData.Now)
                                                .ToList()
                                                .BuildSelectItemList(item => item.Name,
                                                                     item => item.Id.ToString(),
                                                                     emptyItemText: "Please select news list");
        }
    }

    public class LatestXArticlesViewModel
    {
        public IList<Article> Articles { get; set; }
        public string Title { get; set; }
    }

}