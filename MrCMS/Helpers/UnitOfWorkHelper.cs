using System;
using MrCMS.DataAccess;

namespace MrCMS.Helpers
{
    public static class UnitOfWorkHelper
    {
        public static void Transact(this IDbContext context, Action<IDbContext> action)
        {
            using (var unitOfWork = new UnitOfWork(context))
            {
                action(context);
                unitOfWork.Commit();
            }
        }
        public static TResult Transact<TResult>(this IDbContext context, Func<IDbContext, TResult> action)
        {
            using (var unitOfWork = new UnitOfWork(context))
            {
                TResult result = action(context);
                unitOfWork.Commit();
                return result;
            }
        }
    }
}