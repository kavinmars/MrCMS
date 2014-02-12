using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Pages;
using Ninject;

namespace MrCMS.Web.Apps.Galleries.Pages
{
    public class Gallery : TextPage
    {
        public override void AdminViewData(ViewDataDictionary viewData, IKernel kernel)
        {
            viewData["galleries"] = kernel.Get<IDbContext>().Query<MediaCategory>()
                                       .OrderByDescending(category => category.Name)
                                       .ToList()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select a gallery...");
        }

        public virtual MediaCategory MediaGallery { get; set; }

        [DisplayName("Gallery Thumbnail Image")]
        public virtual string ThumbnailImage { get; set; }
    }
}