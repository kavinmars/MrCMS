using System;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;

namespace MrCMS.Services
{
    public class TreeNavService : ITreeNavService
    {
        private readonly IDbContext _dbContext;
        private readonly Site _site;

        public TreeNavService(IDbContext dbContext, Site site)
        {
            _dbContext = dbContext;
            _site = site;
        }

        public AdminTree GetWebpageNodes(int? id)
        {
            var adminTree = new AdminTree();
            var query = _dbContext.Set<Webpage>().Where(x => x.Parent.Id == id && x.Site.Id == _site.Id);
            int maxChildNodes = 1000;
            if (id.HasValue)
            {
                var parent = _dbContext.Get<Webpage>(id.GetValueOrDefault());
                if (parent != null)
                {
                    var metaData = parent.GetMetadata();
                    maxChildNodes = metaData.MaxChildNodes;
                    query = ApplySort(metaData, query);
                }
            }
            else
            {
                adminTree.IsRootRequest = true;
                query = query.OrderBy(x => x.DisplayOrder);
            }

            var rowCount = query.Count();
            query.Take(maxChildNodes).ToList().ForEach(doc =>
                {
                    var documentMetadata = doc.GetMetadata();
                    var node = new AdminTreeNode
                        {
                            Id = doc.Id,
                            ParentId = doc.ParentId,
                            Name = doc.Name,
                            IconClass = documentMetadata.IconClass,
                            NodeType = "Webpage",
                            HasChildren = doc.Children.Any(),
                            Sortable = documentMetadata.Sortable,
                            CanAddChild = doc.GetValidWebpageDocumentTypes().Any(),
                            IsPublished = doc.Published,
                            RevealInNavigation = doc.RevealInNavigation
                        };
                    adminTree.Nodes.Add(node);
                });
            if (rowCount > maxChildNodes)
            {
                adminTree.Nodes.Add(new AdminTreeNode
                    {
                        NumberMore = (rowCount - maxChildNodes),
                        IconClass = "icon-plus",
                        IsMoreLink = true,
                        ParentId = id
                    });
            }
            return adminTree;
        }

        private static IQueryable<Webpage> ApplySort(DocumentMetadata metaData, IQueryable<Webpage> query)
        {
            switch (metaData.SortBy)
            {
                case SortBy.DisplayOrder:
                    query = query.OrderBy(webpage => webpage.DisplayOrder);
                    break;
                case SortBy.DisplayOrderDesc:
                    query = query.OrderByDescending(webpage => webpage.DisplayOrder);
                    break;
                case SortBy.PublishedOn:
                    query =
                        query.OrderByDescending(webpage => webpage.PublishOn == null)
                             .ThenBy(webpage => webpage.PublishOn);
                    break;
                case SortBy.PublishedOnDesc:
                    query =
                    query =
                        query.OrderByDescending(webpage => webpage.PublishOn == null)
                             .ThenByDescending(webpage => webpage.PublishOn);
                    break;
                case SortBy.CreatedOn:
                    query = query.OrderBy(webpage => webpage.CreatedOn);
                    break;
                case SortBy.CreatedOnDesc:
                    query = query.OrderByDescending(webpage => webpage.CreatedOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return query;
        }

        public AdminTree GetMediaCategoryNodes(int? id)
        {
            return GetSimpleAdminTree<MediaCategory>(id, "icon-picture");
        }

        public AdminTree GetLayoutNodes(int? id)
        {
            return GetSimpleAdminTree<Layout>(id, "icon-th-large");
        }

        private AdminTree GetSimpleAdminTree<T>(int? id, string iconClass) where T : Document
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }
            var query =
                _dbContext.Set<T>()
                        .Where(x => x.Parent.Id == id && x.Site.Id == _site.Id)
                        .OrderBy(x => x.DisplayOrder)
                        .ToList();
            query.ForEach(doc =>
                {
                    var node = new AdminTreeNode
                        {
                            Id = doc.Id,
                            ParentId = doc.ParentId,
                            Name = doc.Name,
                            IconClass = iconClass,
                            NodeType = typeof(T).Name,
                            HasChildren = doc.Children.Any(),
                            CanAddChild = true,
                            IsPublished = true,
                            RevealInNavigation = true
                        };
                    adminTree.Nodes.Add(node);
                });
            return adminTree;
        }

    }
}