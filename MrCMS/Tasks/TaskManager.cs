using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using System.Linq;

namespace MrCMS.Tasks
{
    public class TaskManager : ITaskManager
    {
        private readonly IDbContext _dbContext;

        public TaskManager(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ScheduledTask> GetAllScheduledTasks()
        {
            return _dbContext.Set<ScheduledTask>().ToList();
        }

        public IPagedList<QueuedTask> GetQueuedTask(QueuedTaskSearchQuery searchQuery)
        {
            var queryOver = _dbContext.Set<QueuedTask>();

            return queryOver.OrderByDescending(task => task.CreatedOn).Paged(searchQuery.Page);
        }

        public void Add(ScheduledTask scheduledTask)
        {
            _dbContext.Transact(session => session.Add(scheduledTask));
        }

        public void Update(ScheduledTask scheduledTask)
        {
            _dbContext.Transact(session => session.Update(scheduledTask));
        }

        public void Delete(ScheduledTask scheduledTask)
        {
            _dbContext.Transact(session => session.Delete(scheduledTask));
        }
    }
}