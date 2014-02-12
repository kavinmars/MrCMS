using System;
using System.Collections;
using System.Linq;
using Elmah;
using MrCMS.DataAccess;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Logging
{
    public class MrCMSErrorLog : ErrorLog, IDisposable
    {
        private IDbContext _dbContext;

        public override string Name
        {
            get
            {
                return "MrCMS Database Error Log";
            }
        }

        public MrCMSErrorLog(IDictionary config)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
                _dbContext = MrCMSApplication.Get<IDbContext>();
        }

        public override string Log(Error error)
        {
            var newGuid = Guid.NewGuid();

            if (_dbContext != null)
            {
                var log = new Log
                              {
                                  //Error = BinaryData.CanSerialize(error) ? error : new Error(),
                                  Guid = newGuid,
                                  Message = error.Message,
                                  Detail = error.Detail,
                                  Site = _dbContext.Get<Site>(CurrentRequestData.CurrentSite.Id)
                              };
                _dbContext.Transact(session => session.Add(log));
            }

            return newGuid.ToString();
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);

            var errorLogEntries =
                _dbContext.Query<Log>()
                        .OrderByDescending(entry => entry.CreatedOn)
                        .Where(entry => entry.Type == LogEntryType.Error)
                        .Paged(pageIndex + 1, pageSize);
            errorLogEntries.ForEach(entry =>
                                    errorEntryList.Add(new ErrorLogEntry(this, entry.Guid.ToString(), entry.Error)));
            return errorLogEntries.TotalItemCount;
        }

        public override ErrorLogEntry GetError(string id)
        {
            Guid guid;
            try
            {
                guid = new Guid(id);
                id = guid.ToString();
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(ex.Message, id, ex);
            }

            try
            {
                var logEntry = _dbContext.Query<Log>().FirstOrDefault(entry => entry.Guid == guid);
                return new ErrorLogEntry(this, id, logEntry.Error);
            }
            finally
            {
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dbContext != null)
                        _dbContext.Dispose();
                }

                // Indicate that the instance has been disposed.
                _dbContext = null;
                _disposed = true;
            }
        }
    }
}