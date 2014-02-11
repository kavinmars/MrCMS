using System;

namespace MrCMS.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}