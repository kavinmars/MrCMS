using System;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }

    public class UnitOfWork :  IUnitOfWork
    {
        private readonly IDbContext _context;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
            _isNested = CurrentRequestData.CurrentUnitOfWork != null;
        }

        public IDbContext Context
        {
            get { return _context; }
        }

        protected bool IsCommited;
        private readonly bool _isNested;

        public void Dispose()
        {
            if (CurrentRequestData.CurrentUnitOfWork == this)
                CurrentRequestData.CurrentUnitOfWork = null;
        }

        public void Commit()
        {
            if (!_isNested)
            {
                _context.SaveChanges();
                IsCommited = true;
            }
        }


        public void Rollback()
        {
            if (!_isNested)
            {
                
            }
        }

    }
}