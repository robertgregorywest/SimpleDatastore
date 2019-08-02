using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SimpleDatastore
{
    public class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        private static readonly object LockObject = new object();

        private readonly IConfiguration _configuration;

        private readonly IXmlResolver<T> _resolver;

        public string DocumentPath { get; }

        public StorageHelper(IConfiguration configuration, IXmlResolver<T> resolver)
        {
            _configuration = configuration;
            _resolver = resolver;
            DocumentPath = string.Format("{0}\\{1}{2}", _configuration.DatastoreLocation, typeof(T).ToString(), Constants.FileExtension);
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

        /// <summary>
        /// Get a collection of objects from an XML document by using the attribute information
        /// </summary>
        /// <returns>A collection of populated objects from the database</returns>
        public IList<T> GetCollection()
        {
            lock (LockObject)
            {
                var doc = GetDocument();
                var nav = doc.CreateNavigator();

                IList<T> collection = new List<T>();

                if (!nav.MoveToFirstChild()) return collection;

                var iterator = nav.Select(Constants.DataItemName);

                while (iterator.MoveNext())
                {
                    var navCurrent = iterator.Current;
                    var item = _resolver.GetItemFromNode(navCurrent);
                    collection.Add(item);
                }
                return collection;
            }
        }

        /// <summary>
        /// Get a class instance from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the instance</param>
        /// <returns>The instance of T from the XML document</returns>
        public T GetObject(Guid id)
        {
            lock (LockObject)
            {
                var doc = GetDocument();
                var nav = doc.CreateNavigator();

                if (!nav.MoveToFirstChild()) return null;
                var navCurrent =
                    nav.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName,
                        PersistentObject.Identifier, id.ToString()));
                if (navCurrent == null) return null;

                var item = _resolver.GetItemFromNode(navCurrent);

                return item;
            }
        }

        /// <summary>
        /// Saves the instance into XML document using attribute information
        /// </summary>
        /// <param name="instance">the instance of T to be saved</param>
        public void SaveObject(T instance)
        {
            var xmlGenerator = new XmlGenerator<T>(instance);

            var innerXml = xmlGenerator.Create();

            lock (LockObject)
            {
                var doc = GetDocument();

                var objectDocFrag = doc.CreateDocumentFragment();
                objectDocFrag.InnerXml = innerXml;

                var existingNode = doc.SelectExistingNode(instance.Id);

                if (existingNode != null)
                {
                    if (doc.DocumentElement != null) doc.DocumentElement.ReplaceChild(objectDocFrag, existingNode);
                }
                else
                {
                    if (doc.DocumentElement != null) doc.DocumentElement.AppendChild(objectDocFrag);
                }

                SaveDocument(doc);
            }
        }

        /// <summary>
        /// Delete an object from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the object</param>
        public void DeleteObject(Guid id)
        {
            lock (LockObject)
            {
                var doc = GetDocument();

                var objectNode = doc.SelectExistingNode(id);

                if (objectNode == null) return;

                if (doc.DocumentElement != null) doc.DocumentElement.RemoveChild(objectNode);
                SaveDocument(doc);
            }
        }
    }
}
