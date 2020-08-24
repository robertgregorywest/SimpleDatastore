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

        public async Task<string> GetDocumentAsync()
        {
            using (await _lockAsync.ReaderLockAsync().ConfigureAwait(false))
            {
                if (!_fileSystemAsync.File.Exists(_documentPath)) return EmptyDocument();

                await using var stream =
                    _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        public string GetDocument()
        {
            lock (_lock)
            {
                if (!_fileSystem.File.Exists(_documentPath)) return EmptyDocument();

                using var stream =
                    _fileSystem.FileStream.Create(_documentPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public async Task SaveDocumentAsync(string document)
        {
            using (await _lockAsync.WriterLockAsync().ConfigureAwait(false))
            {
                await using var stream = _fileSystemAsync.FileStream.Create(_documentPath, FileMode.Create);
                await using var sr = new StreamWriter(stream);
                await sr.WriteAsync(document);
            }
        }

        public void SaveDocument(string document)
        {
            lock (_lock)
            {
                using var stream = _fileSystem.FileStream.Create(_documentPath, FileMode.Create);
                using var sr = new StreamWriter(stream);
                sr.Write(document);
            }
        }

        private static string EmptyDocument()
        {
            return JsonSerializer.Serialize(new List<T>());
        }
    }
}