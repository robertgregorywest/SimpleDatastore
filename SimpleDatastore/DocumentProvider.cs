using SimpleDatastore.Interfaces;
using System.IO;
using System.Xml;

namespace SimpleDatastore
{
    public class DocumentProvider<T> : IDocumentProvider<T> where T : PersistentObject
    {
        private readonly IConfiguration _config;
        private readonly ICache _cache;
        private readonly string _documentPath;
        private readonly string _keyForDocument;

        public DocumentProvider(IConfiguration config, ICache cache)
        {
            _config = config;
            _cache = cache;
            _documentPath = Path.Combine(_config.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
            _keyForDocument = $"Doc.{typeof(T)}";
        }

        public XmlDocument GetDocument()
        {
            if (_config.EnableCaching)
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

            if (_config.EnableCaching)
            {
                _cache.Set(_keyForDocument, doc, _config.CacheDuration);
            }
            
            return doc;
        }

        public void SaveDocument(XmlDocument document)
        {
            document.Save(_documentPath);

            if (_config.EnableCaching)
            {
                _cache.Remove(_keyForDocument);
            }
        }
    }
}
