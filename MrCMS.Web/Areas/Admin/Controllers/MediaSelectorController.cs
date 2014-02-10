using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaSelectorController : MrCMSAdminController
    {
        private readonly IMediaSelectorService _mediaSelectorService;

        public MediaSelectorController(IMediaSelectorService mediaSelectorService)
        {
            _mediaSelectorService = mediaSelectorService;
        }

        public ActionResult Show(MediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = _mediaSelectorService.GetCategories();
            ViewData["results"] = _mediaSelectorService.Search(searchQuery);
            return PartialView(searchQuery);
        }


        public JsonResult GetFileInfo(string value)
        {
            return Json(_mediaSelectorService.GetFileInfo(value), JsonRequestBehavior.AllowGet);
        }
    }

    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        SelectedItemInfo GetFileInfo(string value);
    }

    public class SelectedItemInfo
    {
        public string Url { get; set; }
    }

    public class MediaSelectorService : IMediaSelectorService
    {
        private readonly IFileService _fileService;
        private readonly IDbContext _dbContext;

        public MediaSelectorService(IDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery)
        {
            var queryOver = _dbContext.Set<MediaFile>();
            if (searchQuery.CategoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == searchQuery.CategoryId);
            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                var term = searchQuery.Query.Trim();
                queryOver =
                    queryOver.Where(
                        file =>
                        file.FileName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        file.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        file.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
            }
            return queryOver.OrderByDescending(file => file.CreatedOn).Paged(searchQuery.Page);
        }

        public List<SelectListItem> GetCategories()
        {
            return _dbContext.Set<MediaCategory>().Where(category => !category.HideInAdminNav).ToList()
                           .BuildSelectItemList(category => category.Name, category => category.Id.ToString(),
                                                emptyItemText: "All categories");
        }

        public SelectedItemInfo GetFileInfo(string value)
        {
            var fileUrl = _fileService.GetFileUrl(value);

            return new SelectedItemInfo
                       {
                           Url = fileUrl
                       };
        }
    }

    public class MediaSelectorSearchQuery
    {
        public MediaSelectorSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public int? CategoryId { get; set; }

        public string Query { get; set; }
    }
}