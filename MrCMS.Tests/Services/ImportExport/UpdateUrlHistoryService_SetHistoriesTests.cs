﻿using System.Collections.Generic;
using FluentAssertions;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateUrlHistoryService_SetHistoriesTests : InMemoryDatabaseTest
    {
        private readonly UpdateUrlHistoryService _updateUrlHistoryService;

        public UpdateUrlHistoryService_SetHistoriesTests()
        {
            _updateUrlHistoryService = new UpdateUrlHistoryService(Session, CurrentSite);
            _updateUrlHistoryService.Initialise();
        }

        [Fact]
        public void AddsANewUrlHistory()
        {
            _updateUrlHistoryService.UrlHistories.Should().HaveCount(0);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string> { "test" } }, new BasicMappedWebpage());

            _updateUrlHistoryService.UrlHistories.Should().HaveCount(1);
            _updateUrlHistoryService.UrlHistories.ElementAt(0).UrlSegment.Should().Be("test");
        }

        [Fact]
        public void AssignsAddedUrlHistoryToTheWebpage()
        {
            _updateUrlHistoryService.UrlHistories.Should().HaveCount(0);
            var basicMappedWebpage = new BasicMappedWebpage();

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string> { "test" } }, basicMappedWebpage);

            _updateUrlHistoryService.UrlHistories.ElementAt(0).Webpage.Should().Be(basicMappedWebpage);
        }

        [Fact]
        public void UnAssigningAUrlHistoryShouldSetTheWebpageToNull()
        {
            var urlHistory = new UrlHistory { UrlSegment = "test" };
            var basicMappedWebpage = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { urlHistory } };
            urlHistory.Webpage = basicMappedWebpage;
            Session.Transact(session => session.Add(urlHistory));
            _updateUrlHistoryService.Initialise();
            _updateUrlHistoryService.UrlHistories.Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string>() }, basicMappedWebpage);

            _updateUrlHistoryService.UrlHistories.ElementAt(0).Webpage.Should().BeNull();
        }

        [Fact]
        public void UnAssigningAUrlHistoryRemoveTheItemFromTheWebpageUrlList()
        {
            var urlHistory = new UrlHistory { UrlSegment = "test" };
            var basicMappedWebpage = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { urlHistory } };
            urlHistory.Webpage = basicMappedWebpage;
            Session.Transact(session => session.Add(urlHistory));
            _updateUrlHistoryService.Initialise();
            basicMappedWebpage.Urls.Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string>() }, basicMappedWebpage);

            basicMappedWebpage.Urls.Should().HaveCount(0);
        }

        [Fact]
        public void MovesTheUrlHistoryBetweenPagesIfTheyAreChanged()
        {
            var urlHistory = new UrlHistory { UrlSegment = "test" };
            var basicMappedWebpage1 = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { urlHistory } };
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { } };
            Session.Transact(session => session.Add(urlHistory));
            Session.Transact(session => session.Add(basicMappedWebpage1));
            Session.Transact(session => session.Add(basicMappedWebpage2));
            _updateUrlHistoryService.Initialise();
            basicMappedWebpage1.Urls.Should().HaveCount(1);
            basicMappedWebpage2.Urls.Should().HaveCount(0);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string>() }, basicMappedWebpage1);
            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string> { "test" } }, basicMappedWebpage2);

            basicMappedWebpage1.Urls.Should().HaveCount(0);
            basicMappedWebpage2.Urls.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldNotCreateNewUrlHistoryWhileMovingUrls()
        {
            var urlHistory = new UrlHistory { UrlSegment = "test" };
            var basicMappedWebpage1 = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { urlHistory } };
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage { Urls = new MrCMSList<UrlHistory> { } };
            Session.Transact(session => session.Add(urlHistory));
            Session.Transact(session => session.Add(basicMappedWebpage1));
            Session.Transact(session => session.Add(basicMappedWebpage2));
            _updateUrlHistoryService.Initialise();
            _updateUrlHistoryService.UrlHistories.Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string>() }, basicMappedWebpage1);
            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO { UrlHistory = new List<string> { "test" } }, basicMappedWebpage2);

            _updateUrlHistoryService.UrlHistories.Should().HaveCount(1);
        }
    }
}