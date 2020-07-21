using SimpleDatastore.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class DocumentProvider<T> : IDocumentProvider<T> where T : PersistentObject
    {
        private readonly IFileSystem _fileSystemAsync;
        private readonly IFileSystem _fileSystem;
        private readonly string _documentPath;
        
        private readonly AsyncReaderWriterLock _lockAsync = new AsyncReaderWriterLock();
        private readonly object _lock = new object();

        public DocumentProvider(IOptions<SimpleDatastoreOptions> options, IHostingEnvironment environment,
            IFileSystem fileSystem)
        {
            _fileSystemAsync = fileSystem;
            _fileSystem = fileSystem;
            _documentPath = Path.Combine(
                environment.ContentRootPath,
                options.Value.DatastoreLocation,
                $"{typeof(T)}{Constants.FileExtension}");
        }

        public async Task<string> GetDocumentAsync()
        {
            using (await _lockAsync.ReaderLockAsync().ConfigureAwait(false))
            {
                if (!_fileSystemAsync.File.Exists(_documentPath))
                {
                    return EmptyDocument();
                }

                return await _fileSystemAsync.File.ReadAllTextAsync(_documentPath).ConfigureAwait(false);
            }
        }

        public string GetDocument()
        {
            lock (_lock)
            {
                return !_fileSystem.File.Exists(_documentPath) ? EmptyDocument() : _fileSystem.File.ReadAllText(_documentPath);
            }
        }

        private static string EmptyDocument()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(Constants.RootElementName)
            ).ToString(SaveOptions.None);
        }

        public async Task SaveDocumentAsync(string document)
        {
            using (await _lockAsync.WriterLockAsync().ConfigureAwait(false))
            {
                await _fileSystemAsync.File.WriteAllTextAsync(_documentPath, document);
            }
        }

        public void SaveDocument(string document)
        {
            lock (_lock)
            {
                _fileSystem.File.WriteAllText(_documentPath, document);
            }
        }
    }
}