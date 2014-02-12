using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.DataAccess;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class WidgetServiceTests : InMemoryDatabaseTest
    {
        private readonly WidgetService _widgetService;

        public WidgetServiceTests()
        {
            _widgetService = new WidgetService(Session, Kernel);
        }
        [Fact]
        public void WidgetService_GetWidget_ReturnsAWidgetWhenIdExists()
        {

            var textWidget = new BasicMappedWidget();
            Session.Transact(session => session.Add(textWidget));

            var loadedWidget = _widgetService.GetWidget<BasicMappedWidget>(textWidget.Id);

            loadedWidget.Should().BeSameAs(textWidget);
        }

        [Fact]
        public void WidgetService_GetWidget_WhenIdIsInvalidShouldReturnNull()
        {
            var loadedWidget = _widgetService.GetWidget<BasicMappedWidget>(-1);

            loadedWidget.Should().BeNull();
        }

        [Fact]
        public void WidgetService_SaveWidget_ShouldAddWidgetToDb()
        {
            _widgetService.SaveWidget(new BasicMappedWidget());

            Session.Query<Widget>().Count().Should().Be(1);
        }

        [Fact]
        public void WidgetService_GetModel_CallsWidgetGetModelOfTheWidgetWithTheSessionOfTheService()
        {
            var model = new object();
            var widget = new WidgetWithModel(model);

            object o = _widgetService.GetModel(widget);

            o.Should().Be(model);
        }

        [Fact]
        public void WidgetService_Delete_RemovesWidgetFromDatabase()
        {
            var widget = new BasicMappedWidget();
            Session.Transact(session => session.Add(widget));

            _widgetService.DeleteWidget(widget);

            Session.Query<Widget>().Count().Should().Be(0);
        }

        [Fact]
        public void WidgetService_Delete_CallsOnDeletingOnTheWidget()
        {
            var widget = A.Fake<Widget>();
            var session = A.Fake<IDbContext>();
            var widgetService = new WidgetService(session, Kernel);

            widgetService.DeleteWidget(widget);

            A.CallTo(() => widget.OnDeleting(session)).MustHaveHappened();
        }
    }

    public class WidgetWithModel : Widget
    {
        private readonly object _model;

        public WidgetWithModel(object model)
        {
            _model = model;
        }
        public override object GetModel(Ninject.IKernel kernel)
        {
            return _model;
        }
    }
}