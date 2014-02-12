using MrCMS.DataAccess;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using Ninject;

namespace MrCMS.Tasks
{
    internal class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(IDbContext dbContext, IKernel kernel)
            : base(dbContext, kernel)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Delete(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Delete; }
        }
    }
}