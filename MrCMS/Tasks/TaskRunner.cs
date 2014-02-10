using System.Linq;
using MrCMS.Website;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskRunner : ITaskRunner
    {
        private readonly ITaskResetter _taskResetter;
        private readonly ITaskQueuer _taskQueuer;
        private readonly ITaskBuilder _taskBuilder;
        private readonly ITaskExecutor _taskExecutor;

        public TaskRunner(ITaskResetter taskResetter, ITaskQueuer taskQueuer, ITaskBuilder taskBuilder, ITaskExecutor taskExecutor)
        {
            _taskResetter = taskResetter;
            _taskQueuer = taskQueuer;
            _taskBuilder = taskBuilder;
            _taskExecutor = taskExecutor;
        }

        public BatchExecutionResult ExecutePendingTasks()
        {
            _taskResetter.ResetHungTasks();

            var pendingQueuedTasks = _taskQueuer.GetPendingQueuedTasks();
            var pendingScheduledTasks = _taskQueuer.GetPendingScheduledTasks();

            var tasksToExecute = _taskBuilder.GetTasksToExecute(pendingQueuedTasks, pendingScheduledTasks);

            return _taskExecutor.Execute(tasksToExecute);
        }
    }

    public interface ITaskResetter
    {
        void ResetHungTasks();
    }

    public class TaskResetter : ITaskResetter
    {
        private readonly IDbContext _dbContext;

        public TaskResetter(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ResetHungTasks()
        {
            _dbContext.Transact(session =>
                                  {
                                      var hungTasks = session.Set<QueuedTask>()
                                                             .Where(
                                                                 task =>
                                                                 (task.Status == TaskExecutionStatus.AwaitingExecution || task.Status == TaskExecutionStatus.Executing) &&
                                                                 task.QueuedAt < CurrentRequestData.Now.AddMinutes(15))
                                                             .ToList();
                                      foreach (var task in hungTasks)
                                      {
                                          task.QueuedAt = null;
                                          task.Status = TaskExecutionStatus.Pending;
                                          session.Update(task);
                                      }
                                  });

        }
    }
}