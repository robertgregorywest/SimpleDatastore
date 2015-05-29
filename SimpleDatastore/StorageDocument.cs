using System.Xml;
using System.IO;

namespace SimpleDatastore
{
    internal class StorageDocument<T> : IStorageDocument<T> where T : PersistentObject
    {
        private readonly IConfiguration _configuration;

        public StorageDocument(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public XmlDocument Get()
        {
            // Create document if it does not exist
            if (!File.Exists(DocumentPath))
            {
                using (XmlWriter writer = XmlWriter.Create(DocumentPath))
                {
                    writer.WriteStartElement(Constants.DataElementName);
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(DocumentPath);
            return doc;
        }

        public void Save(XmlDocument document)
        {
            document.Save(DocumentPath);
        }

        private string DocumentPath
        {
            get
            {
                return string.Format("{0}{1}{2}", _configuration.DatastoreLocation, typeof(T).ToString(), Constants.FileExtension);
            }
        }
    }
}
