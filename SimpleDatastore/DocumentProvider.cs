using SimpleDatastore.Interfaces;
using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace SimpleDatastore
{
    public class DocumentProvider<T> : IDocumentProvider<T> where T : PersistentObject
    {
        private readonly SimpleDatastoreOptions _options;
        private readonly ICache _cache;
        private readonly string _documentPath;
        private readonly string _keyForDocument;

        public DocumentProvider(IHostingEnvironment environment, IOptions<SimpleDatastoreOptions> options, ICache cache)
        {
            _options = options.Value;
            _cache = cache;
            _documentPath = Path.Combine(environment.ContentRootPath, _options.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
            _keyForDocument = $"Doc.{typeof(T)}";
        }

        public XmlDocument GetDocument()
        {
            if (_options.EnableCaching)
            {
                var cacheItem = _cache.Get(_keyForDocument);
                if (cacheItem is XmlDocument docFromCache)
                {
                    return docFromCache;
                }
            }
            
            if (!File.Exists(_documentPath))
            {
                using (var writer = XmlWriter.Create(_documentPath))
                {
                    writer.WriteStartElement(Constants.DataElementName);
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }

            var doc = new XmlDocument();
            doc.Load(_documentPath);

            if (_options.EnableCaching)
            {
                _cache.Set(_keyForDocument, doc, _options.CacheDuration);
            }
            
            return doc;
        }

        public void SaveDocument(XmlDocument document)
        {
            document.Save(_documentPath);

            if (_options.EnableCaching)
            {
                _cache.Remove(_keyForDocument);
            }
        }
    }
}
