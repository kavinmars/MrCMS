using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public class UpdateTagsService : IUpdateTagsService
    {
        private readonly IDbContext _dbContext;
        private readonly Site _site;
        private HashSet<Tag> _tags;

        public UpdateTagsService(IDbContext dbContext, Site site)
        {
            _dbContext = dbContext;
            _site = site;
        }

        public HashSet<Tag> Tags
        {
            get { return _tags; }
        }

        public IUpdateTagsService Inititalise()
        {
            _tags = new HashSet<Tag>(_dbContext.Set<Tag>());
            return this;
        }

        public void SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tagsToAdd = documentDto.Tags.Where(s => !webpage.Tags.Select(tag => tag.Name).Contains(s, StringComparer.InvariantCultureIgnoreCase)).ToList();
            var tagsToRemove = webpage.Tags.Where(tag => !documentDto.Tags.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase)).ToList();
            foreach (var item in tagsToAdd)
            {
                var tag = Tags.FirstOrDefault(t => t.Name.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                if (tag == null)
                {
                    tag = new Tag { Name = item };
                    Tags.Add(tag);
                }
                if (!webpage.Tags.Contains(tag))
                    webpage.Tags.Add(tag);

                if (!tag.Documents.Contains(webpage))
                    tag.Documents.Add(webpage);
            }

            foreach (var tag in tagsToRemove)
            {
                webpage.Tags.Remove(tag);
                tag.Documents.Remove(webpage);
            }
        }

        public void SaveTags()
        {
            _dbContext.Transact(dbContext => Tags.ForEach(tag => dbContext.AddOrUpdate(tag)));
        }
    }
}