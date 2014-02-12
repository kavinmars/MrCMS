using System;
using MrCMS.DataAccess;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskStatusUpdater : ITaskStatusUpdater
    {
        private readonly IDbContext _dbContext;

        public TaskStatusUpdater(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginExecution(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnStarting());
        }

        public void SuccessfulCompletion(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnSuccess());
        }

        public void FailedExecution(IExecutableTask executableTask)
        {
            SetStatus(executableTask, status => status.OnFailure());
        }

        private void SetStatus(IExecutableTask executableTask, Action<IHaveExecutionStatus> action)
        {
            _dbContext.Transact(session =>
                                  {
                                      action(executableTask.Entity);
                                      session.Update(executableTask.Entity as SystemEntity);
                                  });
        }
    }
}