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
        
        private readonly AsyncLock _mutex = new AsyncLock();

        public DocumentProvider(IOptions<SimpleDatastoreOptions> options, IHostingEnvironment environment, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _documentPath = Path.Combine(environment.ContentRootPath, options.Value.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
        }

        public async Task<XmlDocument> GetDocumentAsync()
        {
            using (await _mutex.LockAsync())
            {
                if (!_fileSystem.File.Exists(_documentPath))
                {
<<<<<<< HEAD
                    var root = new XElement(Constants.RootElementName);
                    var output = root.ToString();
                    await _fileSystem.File.WriteAllTextAsync(_documentPath, output, CancellationToken.None);
=======
                    using (var writer = XmlWriter.Create(_documentPath, new XmlWriterSettings { Async = true }))
                    {
                        await writer.WriteStartElementAsync("", Constants.RootElementName, "");
                        await writer.WriteEndElementAsync();
                        await writer.FlushAsync();
                    }
>>>>>>> 3.0.0
                }

                var doc = new XmlDocument();
                doc.Load(_documentPath);

                return doc;
            }
        }

        public async Task SaveDocumentAsync(XmlDocument document)
        {
            using (await _mutex.LockAsync())
            {
                document.Save(_documentPath);
            }
        }
    }
}
