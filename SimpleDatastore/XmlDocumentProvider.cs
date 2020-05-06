using SimpleDatastore.Interfaces;
using System.IO;
using System.Xml;

namespace SimpleDatastore
{
    public class XmlDocumentProvider<T> : IXmlDocumentProvider<T> where T : PersistentObject
    {
        private string DocumentPath { get; }

        public XmlDocumentProvider(IConfiguration configuration)
        {
            DocumentPath = Path.Combine(configuration.DatastoreLocation, $"{typeof(T)}{Constants.FileExtension}");
        }

        public XmlDocument GetDocument()
        {
            // Create document if it does not exist
            if (!File.Exists(DocumentPath))
            {
                using (var writer = XmlWriter.Create(DocumentPath))
                {
                    writer.WriteStartElement(Constants.DataElementName);
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }

            var doc = new XmlDocument();
            doc.Load(DocumentPath);
            return doc;
        }

        public void SaveDocument(XmlDocument document)
        {
            document.Save(DocumentPath);
        }
    }
}
