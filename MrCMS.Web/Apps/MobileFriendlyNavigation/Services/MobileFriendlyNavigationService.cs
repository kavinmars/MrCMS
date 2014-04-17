using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class MobileFriendlyNavigationService : IMobileFriendlyNavigationService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public MobileFriendlyNavigationService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public IEnumerable<MobileFriendlyNavigationRootNode> GetRootNodes()
        {
            QueryOver<Webpage, Webpage> rootIds = QueryOver.Of<Webpage>()
                .Where(node => node.Parent == null && node.Site.Id == _site.Id && node.RevealInNavigation && node.PublishOn != null)
                .Select(node => node.Id);

            IEnumerable<Webpage> rootNodes = _session.QueryOver<Webpage>()
                .WithSubquery.WhereProperty(node => node.Id).In(rootIds)
                .OrderBy(node => node.DisplayOrder).Asc
                .Future<Webpage>();

            ICriterion criterion = Subqueries.WhereProperty<Webpage>(node => node.Parent.Id).In(rootIds);
            IEnumerable<MobileFriendlyNavigationChildNode> childNodes = GetChildNodeTransforms(criterion);

            return rootNodes.Select(root => new MobileFriendlyNavigationRootNode
            {
                Name = root.Name,
                UrlSegment = root.UrlSegment,
                Children = childNodes.Where(x => x.ParentId == root.Id)
            });
        }

        public IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodes(int parentId)
        {
            ICriterion criterion = Restrictions.Where<Webpage>(node => node.Parent.Id == parentId);
            return GetChildNodeTransforms(criterion);
        }

        private IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodeTransforms(ICriterion criterion)
        {
            DetachedCriteria countSubNodes = DetachedCriteria.For<Webpage>("subnode")
                .Add(Restrictions.EqProperty("subnode.Parent.Id", "node.Id"))
                .SetProjection(Projections.Count<Webpage>(subnode => subnode.Id));

            return _session.CreateCriteria<Webpage>("node")
                .Add(Restrictions.Where<Webpage>(node => node.RevealInNavigation && node.PublishOn != null))
                .Add(criterion)
                .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property<Webpage>(node => node.Name), "Name")
                    .Add(Projections.Property<Webpage>(node => node.UrlSegment), "UrlSegment")
                    .Add(Projections.Property<Webpage>(node => node.Parent.Id), "ParentId")
                    .Add(Projections.SubQuery(countSubNodes), "ChildCount"))
                .SetResultTransformer(Transformers.AliasToBean<MobileFriendlyNavigationChildNode>())
                .Future<MobileFriendlyNavigationChildNode>();
        }
    }
}