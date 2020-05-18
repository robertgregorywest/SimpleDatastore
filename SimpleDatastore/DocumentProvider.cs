using SimpleDatastore.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class DocumentProvider<T> : IDocumentProvider<T> where T : PersistentObject
    {
        private readonly string _documentPath;
        
        private readonly AsyncLock _mutex = new AsyncLock();

        public DocumentProvider(IOptions<SimpleDatastoreOptions> options, IHostingEnvironment environment)
        {
            _documentPath = Path.Combine(environment.ContentRootPath, options.Value.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
        }

        public async Task<XmlDocument> GetDocumentAsync()
        {
            using (await _mutex.LockAsync())
            {
                if (!File.Exists(_documentPath))
                {
                    using (var writer = XmlWriter.Create(_documentPath))
                    {
                        await writer.WriteStartElementAsync("", Constants.DataElementName, "");
                        await writer.WriteEndElementAsync();
                        await writer.FlushAsync();
                    }
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
