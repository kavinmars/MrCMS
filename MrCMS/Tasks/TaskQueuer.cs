using System.Collections.Generic;
using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class TaskQueuer : ITaskQueuer
    {
        private readonly IDbContext _dbContext;

        public TaskQueuer(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<QueuedTask> GetPendingQueuedTasks()
        {
            return _dbContext.Transact(session =>
                                         {
                                             var queuedTasks =
                                                 session.Query<QueuedTask>()
                                                        .Where(task => task.Status == TaskExecutionStatus.Pending)
                                                        .ToList();

                                             foreach (var task in queuedTasks)
                                             {
                                                 task.Status = TaskExecutionStatus.AwaitingExecution;
                                                 task.QueuedAt = CurrentRequestData.Now;
                                                 _dbContext.Update(task);
                                             }
                                             return queuedTasks;
                                         });
        }

        public IList<ScheduledTask> GetPendingScheduledTasks()
        {
            return _dbContext.Transact(session =>
                                         {
                                             var scheduledTasks =
                                                 _dbContext.Query<ScheduledTask>().ToList()
                                                         .Where(task =>
                                                             task.Status == TaskExecutionStatus.Pending &&
                                                             (task.LastComplete < CurrentRequestData.Now.AddSeconds(-task.EveryXSeconds) ||
                                                              task.LastComplete == null))
                                                         .ToList();
                                             foreach (var task in scheduledTasks)
                                             {
                                                 task.Status = TaskExecutionStatus.AwaitingExecution;
                                                 _dbContext.Update(task);
                                             }
                                             return scheduledTasks;
                                         });
        }
    }
}