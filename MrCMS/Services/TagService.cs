using System.Collections.Generic;
using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class TagService : ITagService
    {
        private readonly IDbContext _dbContext;

        public TagService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AutoCompleteResult> Search(Document document, string term)
        {
            var tags = GetTags(document);

            return
                _dbContext.Query<Tag>().Where(x => x.Site == document.Site && x.Name.StartsWith(term)).ToList().Select(
                    tag =>
                    new AutoCompleteResult
                        {
                            id = tag.Id,
                            label = string.Format("{0}{1}", tag.Name, (tags.Contains(tag) ? " (Category)" : string.Empty)),
                            value = tag.Name
                        });
        }

        public IEnumerable<Tag> GetTags(Document document)
        {
            ISet<Tag> parentCategories = new HashSet<Tag>();

            if (document != null)
            {
                if (document.Parent != null)
                    parentCategories = document.Parent.Tags;
            }

            return parentCategories;
        }

        public Tag GetByName(string name)
        {
            return _dbContext.Query<Tag>().FirstOrDefault(x => x.Site == CurrentRequestData.CurrentSite
                                                             && x.Name == name);
        }
        public void Add(Tag tag)
        {
            _dbContext.Transact(session => session.Add(tag));
        }
    }
}