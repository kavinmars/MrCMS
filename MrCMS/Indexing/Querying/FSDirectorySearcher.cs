﻿using System.IO;
using Lucene.Net.Store;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Querying
{
    public class FSDirectorySearcher<TEntity, TDefinition> : Searcher<TEntity, TDefinition>
        where TEntity : SystemEntity
        where TDefinition : IIndexDefinition<TEntity>
    {
        private static FSDirectory _directory;

        public FSDirectorySearcher(Site currentSite, IDbContext dbContext, TDefinition definition)
            : base(currentSite, dbContext, definition)
        {
        }

        protected override Directory GetDirectory(Site currentSite)
        {
            return _directory = _directory ?? FSDirectory.Open(new DirectoryInfo(Definition.GetLocation(currentSite)));
        }
    }
}