using System;
using System.Collections.Generic;
using Microsoft.Owin;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing;
using MrCMS.Indexing.Management;
using MrCMS.Tasks;

namespace MrCMS.Web.Apps.Core.Indexing.WebpageSearch
{
    public class UrlSegmentFieldDefinition : StringFieldDefinition<WebpageSearchIndexDefinition, Webpage>
    {
        private readonly IOwinContext _owinContext;

        public UrlSegmentFieldDefinition(ILuceneSettingsService luceneSettingsService,IOwinContext owinContext)
            : base(luceneSettingsService, "urlsegment")
        {
            _owinContext = owinContext;
        }

        protected override IEnumerable<string> GetValues(Webpage obj)
        {
            yield return obj.LiveUrlSegment;
            foreach (UrlHistory urlHistory in obj.Urls)
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
                                                  IndexDefinition = IndexingHelper.Get<AdminWebpageIndexDefinition>(_owinContext)
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