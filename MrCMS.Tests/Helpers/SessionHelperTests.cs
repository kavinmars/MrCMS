using System.Linq;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;
using FluentAssertions;

namespace MrCMS.Tests.Helpers
{
    public class SessionHelperTests : InMemoryDatabaseTest
    {
        [Fact]
        public void SessionHelper_Transact_AllowsNestedCalls()
        {
            var stubWebpage = new StubWebpage();

            this.Invoking(tests => Session.Transact(session => session.Transact(iSession => iSession.Add(stubWebpage))))
                .ShouldNotThrow();
        }

        [Fact]
        public void SessionHelper_Paged_WorksWhenPassedAnIQueryOver()
        {
            Session.Transact(session =>
            {
                for (int i = 0; i < 100; i++)
                {
                    session.Add(new StubWebpage { Name = i.ToString() });
                }
            });

            var pagedList = Session.Query<StubWebpage>().OrderBy(webpage => webpage.Id).Paged(4, 10);

            pagedList.TotalItemCount.Should().Be(100);
            pagedList.Count.Should().Be(10);
            pagedList[0].Id.Should().Be(31);
            pagedList[9].Id.Should().Be(40);
        }

        [Fact]
        public void SessionHelper_Paged_WorksWhenPassedAQueryOver()
        {
            Session.Transact(session =>
            {
                for (int i = 0; i < 100; i++)
                {
                    session.Add(new StubWebpage { Name = i.ToString() });
                }
            });

            var pagedList = Session.Query<StubWebpage>().Paged(4, 10);

            pagedList.TotalItemCount.Should().Be(100);
            pagedList.Count.Should().Be(10);
            pagedList[0].Id.Should().Be(31);
            pagedList[9].Id.Should().Be(40);
        }
    }
}