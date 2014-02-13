using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Threading;
using System.Threading.Tasks;

namespace MrCMS.DataAccess
{
    public class MrCMSExecutionStrategy : IDbExecutionStrategy
    {
        private readonly DefaultExecutionStrategy _defaultExecutionStrategy;
        private readonly SqlAzureExecutionStrategy _sqlAzureExecutionStrategy;

        public MrCMSExecutionStrategy()
        {
            _defaultExecutionStrategy = new DefaultExecutionStrategy();
            _sqlAzureExecutionStrategy = new SqlAzureExecutionStrategy();
        }

        //TODO: make this work out where the current connection is (possibly even just make a setting)
        private bool IsAzure
        {
            get { return false; }
        }

        public void Execute(Action operation)
        {
            if (IsAzure)
                _sqlAzureExecutionStrategy.Execute(operation);
            _defaultExecutionStrategy.Execute(operation);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return IsAzure
                ? _sqlAzureExecutionStrategy.Execute(operation)
                : _defaultExecutionStrategy.Execute(operation);
        }

        public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
        {
            return IsAzure
                ? _sqlAzureExecutionStrategy.ExecuteAsync(operation, cancellationToken)
                : _defaultExecutionStrategy.ExecuteAsync(operation, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return IsAzure
                ? _sqlAzureExecutionStrategy.ExecuteAsync(operation, cancellationToken)
                : _defaultExecutionStrategy.ExecuteAsync(operation, cancellationToken);
        }

        public bool RetriesOnFailure { get { return IsAzure; } }
    }
}