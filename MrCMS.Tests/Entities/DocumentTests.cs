using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Xunit;
using System.Linq;

namespace MrCMS.Tests.Entities
{
    public class DocumentTests : InMemoryDatabaseTest
    {
        [Fact]
        public void Document_CanDelete_IsTrueWhenNoChildren()
        {
            var doc = new StubDocument();

            doc.SetChildren(new List<Document>());

            doc.CanDelete.Should().BeTrue();
        }

        [Fact]
        public void Document_CanDelete_IsFalseWhenChildrenAreAdded()
        {
            var doc = new StubDocument();

            doc.SetChildren(new List<Document> {new StubDocument()});

            doc.CanDelete.Should().BeFalse();
        }

        [Fact]
        public void Document_OnDeleting_RemovesDocumentFromParent()
        {
            var doc = new StubDocument();

            var child = new StubDocument();
            doc.SetChildren(new List<Document> {child});

            child.OnDeleting(Session);

            doc.Children.Should().NotContain(child);
        }

        [Fact]
        public void Document_GetVersions_ReturnsVersionsInDescendingCreatedOnOrder()
        {
            var document = new StubDocument();
            var version1 = new DocumentVersion {CreatedOn = CurrentRequestData.Now};
            var version2 = new DocumentVersion {CreatedOn = CurrentRequestData.Now.AddDays(1)};
            document.SetVersions(new List<DocumentVersion>
                {
                    version1,
                    version2
                });

            var versionsModel = document.GetVersions(1);

            versionsModel.Items.Should().ContainInOrder(new List<DocumentVersion> {version2, version1});
        }

        [Fact]
        public void Document_Grouping_CanGroupByDocumentType()
        {
            var document1 = new BasicMappedNoChildrenInNavWebpage();
            var document2 = new BasicMappedNoChildrenInNavWebpage {PublishOn = DateTime.Today.AddDays(-1)};
            var document3 = new BasicMappedWebpage();

            Session.Transact(session =>
                {
                    session.Add(document1);
                    session.Add(document2);
                    session.Add(document3);
                });

            var list =
                Session.Set<Webpage>()
                       .GroupBy(webpage => webpage.DocumentType)
                       .Select(webpages => new DocumentTypeCount
                           {
                               DocumentType = webpages.Key,
                               Total = webpages.Count(),
                               Unpublished = webpages.Count(webpage => !webpage.Published)
                           });
            list.Should().HaveCount(2);
            list.Sum(count => count.Unpublished).Should().Be(2);
        }
    }
    public class DocumentTypeCount
    {
        public string DocumentType { get; set; }
        public int Total { get; set; }
        public int Unpublished { get; set; }
    }
}