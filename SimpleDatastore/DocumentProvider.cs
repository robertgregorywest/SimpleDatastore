using SimpleDatastore.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class DocumentProvider<T> : IDocumentProvider<T> where T : PersistentObject
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _documentPath;
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public DocumentProvider(IOptions<SimpleDatastoreOptions> options, IHostingEnvironment environment, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _documentPath = Path.Combine(environment.ContentRootPath, options.Value.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
        }

        public async Task<XDocument> GetDocumentAsync()
        {
            using (await _lock.ReaderLockAsync())
            {
                if (!_fileSystem.File.Exists(_documentPath))
                {
                    return new XDocument(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement(Constants.RootElementName)
                    );
                }

                using var reader = _fileSystem.File.OpenText(_documentPath);
                var doc = XDocument.Load(reader);

                return doc;
            }
        }

        public async Task SaveDocumentAsync(XDocument document)
        {
            using (await _lock.WriterLockAsync())
            {
                await using var stream = _fileSystem.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true, Indent = true });
                await document.SaveAsync(writer, CancellationToken.None);
            }
        }
    }
}
