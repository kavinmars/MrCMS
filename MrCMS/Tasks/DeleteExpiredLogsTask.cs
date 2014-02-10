using System.Linq;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class DeleteExpiredLogsTask : SchedulableTask
    {
        private readonly SiteSettings _siteSettings;
        private readonly IDbContextFactory _dbContextFactory;

        public DeleteExpiredLogsTask(SiteSettings siteSettings, IDbContextFactory dbContextFactory)
        {
            _siteSettings = siteSettings;
            _dbContextFactory = dbContextFactory;
        }

        public override int Priority { get { return 0; } }

        protected override void OnExecute()
        {
            using (var session = _dbContextFactory.GetContext())
            {
                var sessionDatas =
                    session.Set<Log>()
                                    .Where(
                                        data =>
                                        data.CreatedOn <= CurrentRequestData.Now.AddDays(-_siteSettings.DaysToKeepLogs))
                                    .ToList();

                using (var transaction = session.Database.BeginTransaction())
                {
                    foreach (var sessionData in sessionDatas)
                    {
                        session.Delete(sessionData);
                    }
                    transaction.Commit();
                }
            }
        }
    }
}