using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate.Mapping;
using Ninject;

namespace MrCMS.Indexing.Definitions
{
    public class UrlSegmentFieldDefinition : StringFieldDefinition<AdminWebpageIndexDefinition, Webpage>
    {
        private readonly IKernel _kernel;

        public UrlSegmentFieldDefinition(ILuceneSettingsService luceneSettingsService, IKernel kernel)
            : base(luceneSettingsService, "urlsegment")
        {
            _kernel = kernel;
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.LiveUrlSegment;
            foreach (var urlHistory in obj.Urls)
            {
                yield return urlHistory.UrlSegment;
            }
        }

        public override Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>> GetRelatedEntities()
        {
            return new Dictionary<Type, Func<SystemEntity, IEnumerable<LuceneAction>>>
                   {
                       {
                           typeof (UrlHistory),
                           entity =>
                           {
                               if (entity is UrlHistory)
                               {
                                   return new List<LuceneAction>
                                          {
                                              new LuceneAction
                                              {
                                                  Entity = (entity as UrlHistory).Webpage,
                                                  Operation = LuceneOperation.Update,
                                                  IndexDefinition = IndexingHelper.Get<AdminWebpageIndexDefinition>(_kernel)
                                              }
                                          };
                               }
                               return new List<LuceneAction>();
                           }
                       }
                   };
        }
    }
}