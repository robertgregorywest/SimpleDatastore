using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string storageDocument = DocumentPath;

            // Create document if it does not exist
            if (!File.Exists(storageDocument))
            {
                using (XmlWriter writer = XmlWriter.Create(storageDocument))
                {
                    writer.WriteStartElement(Constants.DataItemName);
                    writer.WriteEndElement();
                    writer.Flush();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(storageDocument);
            return doc;
        }

        public void Save(System.Xml.XmlDocument document)
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
