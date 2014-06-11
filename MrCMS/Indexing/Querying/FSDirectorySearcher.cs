using System.IO;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Settings;
using NHibernate;
using Ninject;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Querying
{
    public class FSDirectorySearcher<TEntity, TDefinition> : Searcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IndexDefinition<TEntity>
    {
        private FSDirectory _directory;

        public FSDirectorySearcher(Site currentSite, TDefinition definition, SiteSettings siteSettings, IKernel kernel)
            : base(currentSite, definition, siteSettings, kernel)
        {
        }

        protected override Directory GetDirectory(Site site)
        {
            return _directory = _directory ?? FSDirectory.Open(new DirectoryInfo(Definition.GetLocation(site)));
        }
    }
}