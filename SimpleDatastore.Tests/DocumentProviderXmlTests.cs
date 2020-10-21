using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SimpleDatastore.Extensions;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class DocumentProviderXmlTests
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
        public async Task No_document_should_return_empty_document()
        {
            _fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            
            var provider = new DocumentProviderXml<FakeObject, XDocument>(_options, _hostingEnvironment, _fileSystem);

            var doc = await provider.GetDocumentAsync();

            Assert.AreEqual(doc.ToString(), FakeDocuments.EmptyXDocument.ToString());
        }
        
        public async Task Save_document_with_existing_content_should_replace()
        {
            await using var stream = FakeDocuments.CollectionFakeObjectXDocument.ToString().CreateStream();
            
            _fileSystem.FileStream.Create("", FileMode.Create).ReturnsForAnyArgs(stream);
            
            var provider = new DocumentProviderXml<FakeObject, XDocument>(_options, _hostingEnvironment, _fileSystem);

            await provider.SaveDocumentAsync(FakeDocuments.CollectionFakeObjectXDocumentUpdated);

            // TODO: verify the correct content was persisted
        }
    }
}