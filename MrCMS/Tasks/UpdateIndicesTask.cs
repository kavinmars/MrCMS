using MrCMS.DataAccess;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using Ninject;

namespace MrCMS.Tasks
{
    internal class UpdateIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public UpdateIndicesTask(IDbContext dbContext, IKernel kernel)
            : base(dbContext, kernel)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Update(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Update; }
        }
    }
}