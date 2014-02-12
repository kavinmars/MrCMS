using FluentAssertions;
using MrCMS.DataAccess.Mappings;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Messaging;
using Xunit;

namespace MrCMS.Tests.DataAccess
{
    public class DbMappingsTests
    {
        [Fact]
        public void MessageTemplateShouldGetSystemEntityMapping()
        {
            var mapDbModels = DbMappings.GetMappers();

            mapDbModels[typeof(MessageTemplate)].GetType().Should().Be(typeof(DefaultSystemEntityMapping<MessageTemplate>));
        }

        [Fact]
        public void DocumentShouldGetSpecificDocumentMapping()
        {
            var mapDbModels = DbMappings.GetMappers();

            mapDbModels[typeof(Document)].GetType().Should().Be(typeof(DocumentMapping));
        }

        [Fact]
        public void LayoutShouldGetDocumentMapping()
        {
            var mapDbModels = DbMappings.GetMappers();

            mapDbModels[typeof(Layout)].GetType().Should().Be(typeof(DocumentMapping<Layout>));
        }
    }
}