using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Website;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MrCMS.DataAccess
{
    public class LogDocumentVersions : IPreCommitListener
    {
        private readonly IDbContextFactory _dbContextFactory;

        public LogDocumentVersions(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void OnPreCommit(DbChangeTracker tracker)
        {
            foreach (var entry in tracker.Entries()
                        .Where(entry => entry.State == EntityState.Modified).Where(entry => entry.Entity is Webpage))
            {

                if (entry.OriginalValues.PropertyNames.Where(
                    s => !DocumentExtensions.IgnoredVersionPropertyNames.Contains(s))
                    .Any(
                        propertyName =>
                            !object.Equals(entry.OriginalValues.GetValue<object>(propertyName),
                                entry.CurrentValues.GetValue<object>(propertyName))))
                {
                    using (var context = _dbContextFactory.GetContext())
                    {
                        var jObject = new JObject();
                        foreach (var propertyName in entry.OriginalValues.PropertyNames.Where(
                    s => !DocumentExtensions.IgnoredVersionPropertyNames.Contains(s)))
                        {
                            jObject.Add(propertyName,
                                new JRaw(JsonConvert.SerializeObject(entry.OriginalValues[propertyName])));
                        }

                        var document = context.Get<Webpage>((entry.Entity as Webpage).Id);
                        var documentVersion = new DocumentVersion
                                              {
                                                  Document = document,
                                                  Data = JsonConvert.SerializeObject(jObject),
                                                  User = GetUser(context),
                                                  CreatedOn = CurrentRequestData.Now,
                                                  UpdatedOn = CurrentRequestData.Now,
                                                  Site = context.Get<Site>(CurrentRequestData.CurrentSite.Id)
                                              };
                        context.Add(documentVersion);
                        context.SaveChanges();
                    }
                }
            }



        }

        private User GetUser(IDbContext session)
        {
            if (CurrentRequestData.CurrentUser != null)
                return session.Get<User>(CurrentRequestData.CurrentUser.Id);
            if (CurrentRequestData.CurrentContext != null && CurrentRequestData.CurrentContext.User != null)
            {
                return session.Query<User>().FirstOrDefault(user => user.Email == CurrentRequestData.CurrentContext.User.Identity.Name);
            }
            return null;
        }
    }
}