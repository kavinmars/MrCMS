using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;

namespace MrCMS.Entities.Documents
{
    public abstract class Document : SiteEntity
    {
        protected Document()
        {
            Versions = new List<DocumentVersion>();
            Children = new List<Document>();
            Tags = new HashSet<Tag>();
        }
        [Required]
        [StringLength(255)]
        public virtual string Name { get; set; }

        public virtual Document Parent { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public virtual int DisplayOrder { get; set; }

        public virtual string UrlSegment { get; set; }

        public virtual IList<Document> Children { get; set; }

        public virtual ISet<Tag> Tags { get; set; }

        public virtual string TagList
        {
            get { return string.Join(", ", Tags.Select(x => x.Name)); }
        }

        public virtual int ParentId { get { return Parent == null ? 0 : Parent.Id; } }


        public virtual bool CanDelete
        {
            get { return !Children.Any(); }
        }

        protected internal virtual IList<DocumentVersion> Versions { get; set; }

        public virtual VersionsModel GetVersions(int page)
        {
            var documentVersions = Versions.OrderByDescending(version => version.CreatedOn).ToList();
            return
               new VersionsModel(
                   new PagedList<DocumentVersion>(
                       documentVersions, page, 10), Id);
        }

        protected internal virtual void CustomInitialization(IDocumentService service, IDbContext dbContext) { }

        public virtual bool ShowInAdminNav { get { return true; } }

        /// <summary>
        /// Called before a document is to be deleted
        /// Place custom logic in here, or things that cannot be handled by NHibernate due to same table references
        /// </summary>
        public override void OnDeleting(IDbContext dbContext)
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
            }
            base.OnDeleting(dbContext);
        }
    }
}