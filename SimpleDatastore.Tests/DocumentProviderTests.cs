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
        public async Task No_document_should_create_document()
        {
            var options = Substitute.For<IOptions<SimpleDatastoreOptions>>();
            var hostingEnvironment = Substitute.For<IHostingEnvironment>();
            var fileSystem = Substitute.For<IFileSystem>();

            options.Value.Returns(new SimpleDatastoreOptions());
            hostingEnvironment.ContentRootPath.Returns("rootPath");
            fileSystem.File.Exists("").ReturnsForAnyArgs(false);
            //fileSystem.File.Create("").ReturnsForAnyArgs(new MemoryStream());
            
            var provider = new DocumentProvider<FakeObject>(options, hostingEnvironment, fileSystem);

            var document = await provider.GetDocumentAsync();
            
        }
    }
}