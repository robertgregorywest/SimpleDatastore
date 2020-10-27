using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class DocumentProviderTests
    {
        private IOptions<SimpleDatastoreOptions> _options;
        private IHostingEnvironment _hostingEnvironment;
        private IFileSystem _fileSystem;
        
        [SetUp]
        public void SetUp()
        {
            _options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
            _options.Value.Returns(new SimpleDatastoreOptions());
            
            _hostingEnvironment = Substitute.For<IHostingEnvironment>();
            _hostingEnvironment.ContentRootPath.Returns("rootPath");
            _fileSystem = Substitute.For<IFileSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            _options = null;
            _hostingEnvironment = null;
            _fileSystem = null;
        }
        
        [Test]
        public async Task No_document_xml_should_return_empty_xDocument()
        {
            _fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            
            var provider = new DocumentProviderXml<FakeObject, XDocument>(_options, _hostingEnvironment, _fileSystem);

            var doc = await provider.GetDocumentAsync();

            Assert.AreEqual(doc.ToString(), FakeDocuments.EmptyXDocument.ToString());
        }
        
        [Test]
        public async Task No_document_json_should_return_empty_jsonDocument()
        {
            _fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            
            var provider = new DocumentProviderJson<FakeObject, JsonDocument>(_options, _hostingEnvironment, _fileSystem);

            using var actual = await provider.GetDocumentAsync();

            using var expected = FakeDocuments.EmptyJsonDocument;

            Assert.AreEqual(expected.RootElement.ToString(), actual.RootElement.ToString());
        }
    }
}