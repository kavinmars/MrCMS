﻿using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class SearchServiceTests:InMemoryDatabaseTest
    {

        [Fact(Skip = "Need to find a way to test lucene indexes")]
        public void SearchService_SearchDocuments_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            Session.Transact(session =>
                                 {
                                     session.Add(doc1);
                                     session.Add(doc2);
                                 });
            var documentService = GetSearchService();

            var searchResultModels = documentService.SearchDocuments<BasicMappedWebpage>("Test");

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        [Fact(Skip = "Need to find a way to test lucene indexes")]
        public void SearchService_SearchDocumentsDetailed_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            Session.Transact(session =>
                                 {
                                     session.Add(doc1);
                                     session.Add(doc2);
                                 });
            var documentService = GetSearchService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", null);

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        [Fact(Skip = "Need to find a way to test lucene indexes")]
        public void SearchService_SearchDocumentsDetailed_FiltersByParentIfIdIsSet()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            var doc3 = new BasicMappedWebpage { Name = "Another Name" };
            Session.Transact(session =>
                                 {
                                     doc1.Parent = doc2;
                                     session.Add(doc1);
                                     session.Add(doc2);
                                     session.Add(doc3);
                                 });
            var documentService = GetSearchService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc3.Id);

            searchResultModels.Should().HaveCount(0);
        }

        [Fact(Skip = "Need to find a way to test lucene indexes")]
        public void SearchService_SearchDocumentsDetailed_FiltersByParentIfIdIsSetReturnsIfItIsCorrect()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            var doc3 = new BasicMappedWebpage { Name = "Another Name" };
            Session.Transact(session =>
                                 {
                                     doc1.Parent = doc2;
                                     session.Add(doc1);
                                     session.Add(doc2);
                                     session.Add(doc3);
                                 });
            var documentService = GetSearchService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc2.Id);

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        private SearchService GetSearchService()
        {
            return null;// new SearchService( new CurrentSite(CurrentSite));
        }
    }
}