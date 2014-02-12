using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class UrlHistoryServiceTests:InMemoryDatabaseTest
    {
        private readonly UrlHistoryService _urlHistoryService;

        public UrlHistoryServiceTests()
        {
            _urlHistoryService = new UrlHistoryService(Session);
        }

        [Fact]
        public void UrlHistoryService_Add_AddsAHistoryToTheDb()
        {
            _urlHistoryService.Add(new UrlHistory {Webpage = new StubWebpage()});

            Session.Query<UrlHistory>().Count().Should().Be(1);
        }

        [Fact]
        public void UrlHistoryService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            Session.Transact(session => session.Add(urlHistory));

            _urlHistoryService.Delete(urlHistory);

            Session.Query<UrlHistory>().ToList().Should().NotContain(urlHistory);
        }
    }
}