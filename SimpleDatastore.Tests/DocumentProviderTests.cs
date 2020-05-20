using System.IO;
using System.IO.Abstractions;
using System.Threading;
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
        public async Task No_document_should_return_empty_document()
        {
            _fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            
            var provider = new DocumentProvider<FakeObject>(_options, _hostingEnvironment, _fileSystem);

            var doc = await provider.GetDocumentAsync();

            Assert.AreEqual(doc.ToString(), FakeDocuments.EmptyXDocument.ToString());
        }
        
        public async Task Save_document_with_existing_content_should_replace()
        {
            var document = Substitute.For<XDocument>();

            await using var stream = GenerateStreamFromString(FakeDocuments.CollectionFakeObjectXDocument.ToString());
            
            _fileSystem.FileStream.Create("", FileMode.Create).ReturnsForAnyArgs(stream);
            
            var provider = new DocumentProvider<FakeObject>(_options, _hostingEnvironment, _fileSystem);

            await provider.SaveDocumentAsync(document);

            // TODO: verify the correct content was persisted
        }
        
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}