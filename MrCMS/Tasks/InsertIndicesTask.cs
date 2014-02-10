using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using Ninject;

namespace MrCMS.Tasks
{
    internal class InsertIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public InsertIndicesTask(IDbContext dbContext, IKernel kernel)
            : base(dbContext, kernel)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Insert(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Insert; }
        }
    }
}