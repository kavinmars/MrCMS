﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.DataAccess;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaCategory : Document
    {
        public MediaCategory()
        {
            Files = new MrCMSList<MediaFile>();
        }
        [Required]
        [Remote("ValidateUrlIsAllowed", "MediaCategory", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage = "Path must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Path")]
        public override string UrlSegment { get; set; }

        public virtual string MetaTitle { get; set; }
        public virtual string MetaDescription { get; set; }

        public virtual bool IsGallery { get { return true; }}


        public virtual MrCMSList<MediaFile> Files { get; set; }

        public virtual bool HideInAdminNav { get; set; }
        public override bool ShowInAdminNav { get { return !HideInAdminNav; } }
        
        public override void OnDeleting(IDbContext dbContext)
        {
            base.OnDeleting(dbContext);

            var mediaFiles = Files.ToList();

            var fileService = MrCMSApplication.Get<IFileService>();
            foreach (var mediaFile in mediaFiles)
                fileService.DeleteFile(mediaFile);
            fileService.RemoveFolder(this);
        }
    }
}
