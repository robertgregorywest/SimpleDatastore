using SimpleDatastore.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    // ReSharper disable once UnusedTypeParameter
    // Not closing the type so that DI container can resolve correctly
    public class DocumentProviderXml<T, TKey, TDocument> : IDocumentProvider<T, TKey, XDocument>
        where T : PersistentObject<TKey>
        where TKey : struct
        where TDocument : XDocument
    {
        private readonly IFileSystem _fileSystemAsync;
        private readonly IFileSystem _fileSystem;
        private readonly string _documentPath;
        private readonly AsyncReaderWriterLock _lockAsync = new AsyncReaderWriterLock();
        private readonly object _lock = new object();

        public DocumentProviderXml(IOptions<SimpleDatastoreOptions> options,
            IHostingEnvironment environment,
            IFileSystem fileSystem)
        {
            _fileSystemAsync = fileSystem;
            _fileSystem = fileSystem;
            _documentPath = Path.Combine(
                environment.ContentRootPath,
                options.Value.DatastoreLocation,
                $"{typeof(T)}.xml");
        }

        public async Task<XDocument> GetDocumentAsync()
        {
            using (await _lockAsync.ReaderLockAsync().ConfigureAwait(false))
            {
                if (!_fileSystemAsync.File.Exists(_documentPath)) return EmptyDocument;

                await using var stream =
                    _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        public XDocument GetDocument()
        {
            lock (_lock)
            {
                if (!_fileSystem.File.Exists(_documentPath)) return EmptyDocument;

                using var stream =
                    _fileSystem.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return XDocument.Load(stream, LoadOptions.None);
            }
        }

        public async Task SaveDocumentAsync(XDocument document)
        {
            using (await _lockAsync.WriterLockAsync().ConfigureAwait(false))
            {
                await using var stream = _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = XmlWriter.Create(stream, new XmlWriterSettings {Async = true, Indent = true});
                await document.SaveAsync(writer, CancellationToken.None).ConfigureAwait(false);
            }
        }

        public void SaveDocument(XDocument document)
        {
            lock (_lock)
            {
                using var stream = _fileSystem.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = XmlWriter.Create(stream, new XmlWriterSettings {Indent = true});
                document.Save(writer);
            }
        }

        private static XDocument EmptyDocument => new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(PersistentObject.RootElementName)
        );
    }
}