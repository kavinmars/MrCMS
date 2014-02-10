using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WebpageControllerTests : MrCMSTest
    {
        private readonly IDocumentService documentService;
        private readonly IFormService formService;
        private readonly IDbContext session;
        private readonly WebpageController webpageController;
        private readonly Site _site = new Site();

        public WebpageControllerTests()
        {
            CurrentRequestData.CurrentUser = new User();
            documentService = A.Fake<IDocumentService>();
            formService = A.Fake<IFormService>();
            webpageController = new WebpageController(documentService, formService, Kernel, _site)
            {
                RouteDataMock = new RouteData()
            };

            DocumentMetadataHelper.OverrideExistAny = type => false;
        }

        [Fact]
        public void WebpageController_AddGet_ShouldReturnAddPageModel()
        {
            var actionResult = webpageController.Add_Get(1) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var textPage = new TextPage { Site = _site, Id = 1 };
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(textPage);

            var actionResult = webpageController.Add_Get(1) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void WebpageController_AddGet_ShouldSetViewDataToSelectListItem()
        {
            var textPage = new TextPage { Site = _site };
            A.CallTo(() => documentService.GetDocument<Document>(1)).Returns(textPage);

            webpageController.Add_Get(1);

            webpageController.ViewData["Layout"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }

        [Fact]
        public void WebpageController_AddPost_ShouldCallSaveDocument()
        {
            var webpage = new TextPage { Site = _site };
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, null)).Returns(true);

            webpageController.Add(webpage); 

            A.CallTo(() => documentService.AddDocument<Webpage>(webpage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_AddPost_ShouldRedirectToEdit()
        {
            var webpage = new TextPage { Id = 1 };
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, null)).Returns(true);

            var result = webpageController.Add(webpage) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnViewResult()
        {
            var webpage = new TextPage { Id = 1 };
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, null)).Returns(false);

            var result = webpageController.Add(webpage);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_AddPost_IfIsValidForWebpageIsFalseShouldReturnPassedObjectAsModel()
        {
            var webpage = new TextPage { Id = 1 };
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, null)).Returns(false);

            var result = webpageController.Add(webpage);

            result.As<ViewResult>().Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnAViewResult()
        {
            ActionResult result = webpageController.Edit_Get(new TextPage());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var webpage = new TextPage { Id = 1 };

            var result = webpageController.Edit_Get(webpage) as ViewResult;

            result.Model.Should().Be(webpage);
        }

        [Fact]
        public void WebpageController_EditGet_ShouldCallGetAllLayouts()
        {
            webpageController.Edit_Get(new TextPage());

            A.CallTo(() => documentService.GetAllDocuments<Layout>()).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetViewDataToSelectListItem()
        {
            var result = webpageController.Edit_Get(new TextPage()) as ViewResult;

            webpageController.ViewData["Layout"].Should().BeAssignableTo<IEnumerable<SelectListItem>>();
        }

        [Fact]
        public void WebpageController_EditGet_ShouldSetLayoutDetailsToSelectListItems()
        {
            var layout = new Layout { Id = 1, Name = "Layout Name", Site = _site };
            A.CallTo(() => documentService.GetAllDocuments<Layout>()).Returns(new List<Layout> { layout });

            webpageController.Edit_Get(new TextPage());

            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>()
                                                .Skip(1)
                                                .First()
                                                .Selected.Should()
                                                .BeFalse();
            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>()
                                                .Skip(1)
                                                .First()
                                                .Text.Should()
                                                .Be("Layout Name");
            webpageController.ViewData["Layout"].As<IEnumerable<SelectListItem>>()
                                                .Skip(1)
                                                .First()
                                                .Value.Should()
                                                .Be("1");
        }

        [Fact]
        public void WebpageController_EditPost_ShouldCallSaveDocument()
        {
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, 1)).Returns(true);
            Webpage textPage = new TextPage { Id = 1 };

            webpageController.Edit(textPage);

            A.CallTo(() => documentService.SaveDocument(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_EditPost_ShouldRedirectToEdit()
        {
            A.CallTo(() => documentService.UrlIsValidForWebpage(null, 1)).Returns(true);
            var textPage = new TextPage { Id = 1 };

            ActionResult actionResult = webpageController.Edit(textPage);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var textPage = new TextPage();

            webpageController.Sort(textPage);

            A.CallTo(() => documentService.GetDocumentsByParent<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Sort_ShouldBeAListOfSortItems()
        {
            var textPage = new TextPage();
            var webpages = new List<Webpage> { new TextPage() };
            A.CallTo(() => documentService.GetDocumentsByParent<Webpage>(textPage)).Returns(webpages);

            var viewResult = webpageController.Sort(textPage).As<ViewResult>();

            viewResult.Model.Should().BeOfType<List<SortItem>>();
        }

        [Fact]
        public void WebpageController_View_InvalidIdReturnsRedirectToIndex()
        {
            A.CallTo(() => documentService.GetDocument<Webpage>(1)).Returns(null);

            ActionResult actionResult = webpageController.Show(null);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = webpageController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var textPage = new TextPage();

            webpageController.SuggestDocumentUrl(textPage, "test");

            A.CallTo(() => documentService.GetDocumentUrl("test", textPage, true)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var textPage = new TextPage();
            A.CallTo(() => documentService.GetDocumentUrl("test", textPage, true)).Returns("test/result");

            string url = webpageController.SuggestDocumentUrl(textPage, "test");

            url.Should().BeEquivalentTo("test/result");
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsPartialViewResult()
        {
            webpageController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_DeleteGet_ReturnsDocumentPassedAsModel()
        {
            var textPage = new TextPage();

            webpageController.Delete_Get(textPage).As<PartialViewResult>().Model.Should().Be(textPage);
        }

        [Fact]
        public void WebpageController_Delete_ReturnsRedirectToIndex()
        {
            var stubWebpage = new StubWebpage();

            webpageController.Delete(stubWebpage).Should().BeOfType<RedirectToRouteResult>();
            webpageController.Delete(stubWebpage).As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_Delete_CallsDeleteDocumentOnThePassedObject()
        {
            var textPage = new TextPage();
            webpageController.Delete(textPage);

            A.CallTo(() => documentService.DeleteDocument<Webpage>(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_PublishNow_ReturnsRedirectToRouteResult()
        {
            var textPage = new TextPage { Id = 1 };
            webpageController.PublishNow(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_PublishNow_RedirectsToEditForId()
        {
            var textPage = new TextPage { Id = 1 };
            var result = webpageController.PublishNow(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_PublishNow_CallsDocumentServicePublishNow()
        {
            var textPage = new TextPage();

            webpageController.PublishNow(textPage);

            A.CallTo(() => documentService.PublishNow(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Unpublish_ReturnsRedirectToRouteResult()
        {
            var textPage = new TextPage { Id = 1 };
            webpageController.Unpublish(textPage).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_Unpublish_RedirectsToEditForId()
        {
            var textPage = new TextPage { Id = 1 };
            var result = webpageController.Unpublish(textPage).As<RedirectToRouteResult>();

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WebpageController_Unpublish_CallsDocumentServicePublishNow()
        {
            var textPage = new TextPage();
            webpageController.Unpublish(textPage);

            A.CallTo(() => documentService.Unpublish(textPage)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ViewChanges_ShouldReturnPartialViewResult()
        {
            var documentVersion = new DocumentVersion();

            webpageController.ViewChanges(documentVersion).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_ViewChanges_NullDocumentVersionRedirectsToIndex()
        {
            ActionResult result = webpageController.ViewChanges(null);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void WebpageController_ViewPosting_ShouldReturnAPartialViewResult()
        {
            webpageController.ViewPosting(new FormPosting()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_ViewPosting_ReturnsTheResultOfTheCallToGetFormPostingAsTheModel()
        {
            var formPosting = new FormPosting();
            webpageController.ViewPosting(formPosting).As<PartialViewResult>().Model.Should().Be(formPosting);
        }

        [Fact]
        public void WebpageController_Postings_ReturnsAPartialViewResult()
        {
            webpageController.Postings(new TextPage(), 1, null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_Postings_CallsFormServiceGetFormPostingsWithPassedArguments()
        {
            var textPage = new TextPage();
            webpageController.Postings(textPage, 1, null);

            A.CallTo(() => formService.GetFormPostings(textPage, 1, null)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_Posting_ReturnsTheResultOfTheCallToGetFormPostings()
        {
            var textPage = new TextPage();
            var postingsModel = new PostingsModel(new PagedList<FormPosting>(new FormPosting[0], 1, 1), 1);
            A.CallTo(() => formService.GetFormPostings(textPage, 1, null)).Returns(postingsModel);
            webpageController.Postings(textPage, 1, null).As<PartialViewResult>().Model.Should().Be(postingsModel);
        }

        [Fact]
        public void WebpageController_Versions_ReturnsPartialViewResult()
        {
            var document = new StubDocument();
            document.SetVersions(new List<DocumentVersion>());
            webpageController.Versions(document, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WebpageController_Versions_CallsGetVersionsOnPassedDocument()
        {
            var document = A.Fake<StubDocument>();
            document.SetVersions(new List<DocumentVersion>());
            webpageController.Versions(document, 1);

            A.CallTo(() => document.GetVersions(1)).MustHaveHappened();
        }


        [Fact]
        public void WebpageController_HideWidget_CallsDocumentServiceHideWidgetWithPassedArguments()
        {
            var stubWebpage = new StubWebpage();

            webpageController.HideWidget(stubWebpage, 2, 3);

            A.CallTo(() => documentService.HideWidget(stubWebpage, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_HideWidget_ReturnsARedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            webpageController.HideWidget(stubWebpage, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_HideWidget_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var stubWebpage = new StubWebpage { Id = 1 };

            var redirectToRouteResult = webpageController.HideWidget(stubWebpage, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(stubWebpage.Id);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }

        [Fact]
        public void WebpageController_ShowWidget_CallsDocumentServiceShowWidgetWithPassedArguments()
        {
            var stubWebpage = new StubWebpage();

            webpageController.ShowWidget(stubWebpage, 2, 3);

            A.CallTo(() => documentService.ShowWidget(stubWebpage, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_ShowWidget_ReturnsARedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            webpageController.ShowWidget(stubWebpage, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageController_ShowWidget_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var stubWebpage = new StubWebpage { Id = 1 };

            var redirectToRouteResult = webpageController.ShowWidget(stubWebpage, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(stubWebpage.Id);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }

        ~WebpageControllerTests()
        {
            DocumentMetadataHelper.OverrideExistAny = null;
        }
    }
}