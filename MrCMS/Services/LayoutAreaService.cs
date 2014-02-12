using System.Linq;
using MrCMS.DataAccess;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Helpers;
using System.Data.Entity;

namespace MrCMS.Services
{
    public class LayoutAreaService : ILayoutAreaService
    {
        private readonly IDbContext _dbContext;

        public LayoutAreaService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public LayoutArea GetArea(Layout layout, string name)
        {
            return _dbContext.Query<LayoutArea>().Where(x => x.Layout == layout && x.AreaName == name).Include(
                area => area.Widgets).FirstOrDefault();
        }

        public void SaveArea(LayoutArea layoutArea)
        {
            _dbContext.Transact(session =>
                                  {
                                      var layout = layoutArea.Layout;
                                      if (layout != null)
                                      {
                                          layout.LayoutAreas.Add(layoutArea);
                                      }
                                      session.AddOrUpdate(layoutArea);
                                  });
        }

        public LayoutArea GetArea(int layoutAreaId)
        {
            return _dbContext.Get<LayoutArea>(layoutAreaId);
        }

        public void DeleteArea(LayoutArea area)
        {
            _dbContext.Transact(session =>
                                  {
                                      if (area.Layout != null)
                                          area.Layout.LayoutAreas.Remove(area); //required to clear cache
                                      session.Delete(area);
                                  });
        }

        public void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _dbContext.Transact(session => pageWidgetSortModel.Widgets.ForEach(model =>
                                                                                 {
                                                                                     var widget = _dbContext.Get<Widget>(model.Id);
                                                                                     widget.DisplayOrder = model.Order;
                                                                                     session.Update(widget);
                                                                                 }));
        }

        public void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel)
        {
            _dbContext.Transact(session =>
            {

                var layoutArea = _dbContext.Get<LayoutArea>(pageWidgetSortModel.LayoutAreaId);
                var webpage = _dbContext.Get<Webpage>(pageWidgetSortModel.WebpageId);
                pageWidgetSortModel.Widgets.ForEach(model =>
                    {
                        var widget = _dbContext.Get<Widget>(model.Id);

                        var widgetSort =
                            _dbContext.Query<PageWidgetSort>().SingleOrDefault(sort => sort.LayoutArea == layoutArea &&
                                                                                     sort.Webpage == webpage &&
                                                                                     sort.Widget == widget) ??
                            new PageWidgetSort
                                {
                                    LayoutArea =
                                        layoutArea,
                                    Webpage = webpage,
                                    Widget = widget
                                };
                        widgetSort.Order = model.Order;
                        if (!layoutArea.PageWidgetSorts.Contains(widgetSort))
                            layoutArea.PageWidgetSorts.Add(widgetSort);
                        if (!webpage.PageWidgetSorts.Contains(widgetSort))
                            webpage.PageWidgetSorts.Add(widgetSort);
                        if (!widget.PageWidgetSorts.Contains(widgetSort))
                            widget.PageWidgetSorts.Add(widgetSort);
                        session.AddOrUpdate(widgetSort);
                    });

            });
        }

        public void ResetSorting(LayoutArea area, Webpage webpage)
        {
            var list = webpage.PageWidgetSorts.Where(sort => sort.LayoutArea == area).ToList();

            _dbContext.Transact(session => list.ForEach(sort =>
                                                          {
                                                              sort.OnDeleting(session);
                                                              session.Delete(sort);
                                                          }));
        }
    }
}