using System.Collections.Generic;
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
    // ReSharper disable once UnusedTypeParameter
    // Not closing the type so that DI container can resolve correctly
    public class DocumentProviderJson<T, TDocument> : IDocumentProvider<T, JsonDocument> 
        where T : PersistentObject
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
                await using var stream = _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Create);
                await using var writer = new Utf8JsonWriter(stream);
                document.WriteTo(writer);
            }
        }

        public void SaveDocument(JsonDocument document)
        {
            lock (_lock)
            {
                using var stream = _fileSystem.FileStream.Create(_documentPath, FileMode.Create);
                using var writer = new Utf8JsonWriter(stream);
                document.WriteTo(writer);
            }
        }

        private static JsonDocument EmptyDocument() => JsonDocument.Parse(JsonSerializer.Serialize(new List<T>()));
    }
}