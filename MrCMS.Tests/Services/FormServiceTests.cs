using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.DataAccess.CustomCollections;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class FormServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;
        private readonly FormService _formService;

        public FormServiceTests()
        {
            _documentService = A.Fake<DocumentService>();
            _fileService = A.Fake<FileService>();
            _siteSettings = A.Fake<SiteSettings>();
            _mailSettings = A.Fake<MailSettings>();
            _formService = new FormService(Session,_documentService, _fileService, _siteSettings, _mailSettings);
        }

        [Fact]
        public void FormService_ClearFormData_ShouldDeleteFormPosting()
        {
            var webpage = new BasicMappedWebpage();
            var posting = new FormPosting()
                {
                    Webpage = webpage,
                    FormValues = new MrCMSList<FormValue>()
                        {
                            new FormValue()
                                {
                                    IsFile = false,
                                    Key = "Name",
                                    Value = "MrCMS"
                                }
                        }
                };

            webpage.FormPostings = new MrCMSList<FormPosting>() { posting };

            Session.Transact(session => session.Add(posting));

            _formService.ClearFormData(webpage);

            Session.Query<FormPosting>().Count().Should().Be(0);
        }

        [Fact]
        public void FormService_ExportFormData_ShouldReturnByteArray()
        {
            var webpage = new BasicMappedWebpage();
            var posting = new FormPosting()
            {
                Webpage = webpage,
                FormValues = new MrCMSList<FormValue>()
                        {
                            new FormValue()
                                {
                                    IsFile = false,
                                    Key = "Name",
                                    Value = "MrCMS"
                                }
                        }
            };

            webpage.FormPostings = new MrCMSList<FormPosting>() { posting };

            var result=_formService.ExportFormData(webpage);

            result.Should().BeOfType<byte[]>();
        }
    }
}