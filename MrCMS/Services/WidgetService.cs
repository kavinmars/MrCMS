using MrCMS.DataAccess;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services
{
    public class WidgetService : IWidgetService
    {
        private readonly IDbContext _dbContext;
        private readonly IKernel _kernel;

        public WidgetService(IDbContext dbContext,IKernel kernel)
        {
            _dbContext = dbContext;
            _kernel = kernel;
        }

        public T GetWidget<T>(int id) where T : Widget
        {
            return _dbContext.Get<T>(id);
        }

        public void SaveWidget(Widget widget)
        {
            _dbContext.Transact(session => session.AddOrUpdate(widget));
        }

        public object GetModel(Widget widget)
        {
            return widget.GetModel(_kernel);
        }

        public void DeleteWidget(Widget widget)
        {
            _dbContext.Transact(session =>
                {
                    widget.OnDeleting(session);
                    session.Delete(widget);
                });
        }

        public Widget AddWidget(Widget widget)
        {
            return _dbContext.Transact(session => session.AddOrUpdate(widget));
        }
    }
}