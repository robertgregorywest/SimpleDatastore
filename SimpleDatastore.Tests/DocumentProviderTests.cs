using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace SimpleDatastore.Tests
{
    [TestFixture]
    public class DocumentProviderTests
    {
        [Test]
        public async Task No_document_should_return_empty_document()
        {
            var options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
            var hostingEnvironment = Substitute.For<IHostingEnvironment>();
            var fileSystem = Substitute.For<IFileSystem>();

            options.Value.Returns(new SimpleDatastoreOptions());
            hostingEnvironment.ContentRootPath.Returns("rootPath");
            fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            
            var provider = new DocumentProvider<FakeObject>(options, hostingEnvironment, fileSystem);

            var doc = await provider.GetDocumentAsync();

            Assert.AreEqual(doc.ToString(), FakeDocuments.EmptyXDocument.ToString());
        }
    }
}