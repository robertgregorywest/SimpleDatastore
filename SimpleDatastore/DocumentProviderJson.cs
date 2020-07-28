using SimpleDatastore.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class DocumentProviderJson<T> : IDocumentProviderJson<T> where T : PersistentObject
    {
        private readonly IFileSystem _fileSystemAsync;
        private readonly IFileSystem _fileSystem;
        private readonly string _documentPath;
        private readonly AsyncReaderWriterLock _lockAsync = new AsyncReaderWriterLock();
        private readonly object _lock = new object();

        public DocumentProviderJson(IOptions<SimpleDatastoreOptions> options,
            IHostingEnvironment environment,
            IFileSystem fileSystem)
        {
            _fileSystemAsync = fileSystem;
            _fileSystem = fileSystem;
            _documentPath = Path.Combine(environment.ContentRootPath, options.Value.DatastoreLocation,
                $"{typeof(T)}.json");
        }

        public async Task<JsonDocument> GetDocumentAsync()
        {
            using (await _lockAsync.ReaderLockAsync().ConfigureAwait(false))
            {
                if (!_fileSystemAsync.File.Exists(_documentPath)) return EmptyDocument();

                await using var stream =
                    _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return await JsonDocument.ParseAsync(stream);
            }
        }

        public JsonDocument GetDocument()
        {
            lock (_lock)
            {
                if (!_fileSystem.File.Exists(_documentPath)) return EmptyDocument();

                using var stream =
                    _fileSystem.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return JsonDocument.Parse(stream);
            }
        }

        public async Task SaveDocumentAsync(JsonDocument document)
        {
            using (await _lockAsync.WriterLockAsync().ConfigureAwait(false))
            {
                var writerOptions = new JsonWriterOptions
                {
                    Indented = true
                };
                
                await using var stream = _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = new Utf8JsonWriter(stream, options: writerOptions);
                document.WriteTo(writer);
            }
        }

        public void SaveDocument(JsonDocument document)
        {
            lock (_lock)
            {
                var writerOptions = new JsonWriterOptions
                {
                    Indented = true
                };
                
                using var stream = _fileSystem.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = new Utf8JsonWriter(stream, options: writerOptions);
                document.WriteTo(writer);
            }
        }

        private static JsonDocument EmptyDocument()
        {
            return JsonDocument.Parse("[]");
        }
    }
}