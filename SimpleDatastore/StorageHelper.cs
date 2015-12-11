using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleDatastore
{
    internal class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        private static readonly object LockObject = new object();

        private readonly IDependencyResolver _resolver;
        private readonly IStorageDocument<T> _storageDocument;

        public StorageHelper(IDependencyResolver resolver, IStorageDocument<T> storageDocument)
        {
            _resolver = resolver;
            _storageDocument = storageDocument;
        }

        /// <summary>
        /// Get a collection of objects from an XML document by using the attribute information
        /// </summary>
        /// <returns>A collection of populated objects from the database</returns>
        public IList<T> GetCollection()
        {
            var doc = _storageDocument.Get();
            var nav = doc.CreateNavigator();

            IList<T> collection = new List<T>();

            if (!nav.MoveToFirstChild()) return collection;
            var iterator = nav.Select(Constants.DataItemName);

            while (iterator.MoveNext())
            {
                var navCurrent = iterator.Current;
                var item = GetItemFromNode(navCurrent);
                collection.Add(item);
            }
            return collection;
        }

        /// <summary>
        /// Get a class instance from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the instance</param>
        /// <returns>The instance of T from the XML document</returns>
        public T GetObject(Guid id)
        {
            var doc = _storageDocument.Get();
            var nav = doc.CreateNavigator();

            if (!nav.MoveToFirstChild()) return null;
            var navCurrent = nav.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, id.ToString()));
            if (navCurrent == null) return null;
            var item = GetItemFromNode(navCurrent);
            return item;
        }

        private T GetItemFromNode(XPathNavigator nav)
        {
            // Using the resolver so that objects can have dependencies
            var instance = _resolver.GetService<T>();

            foreach (var property in typeof(T).GetValidProperties().Where(property => nav.MoveToChild(property.GetPropertyName(), "")))
            {
                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(instance, nav.Value, null);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    var guid = new Guid(nav.Value);
                    property.SetValue(instance, guid, null);
                }
                else if (property.PropertyType.IsAPersistentObject())
                {
                    var repositoryType = typeof(BaseRepository<>).MakeGenericType(property.PropertyType);
                    dynamic repository = _resolver.GetService(repositoryType);
                    var persistentObject = repository.Load(nav.Value.ToGuid());
                    property.SetValue(instance, persistentObject, null);
                }
                else if (property.PropertyType.IsAPersistentObjectList())
                {
                    var persistentObjectIds = nav.Value.Split(',');
                    var elementType = property.PropertyType.GetGenericArguments()[0];
                    var mapperType = typeof(CollectionMapper<>).MakeGenericType(elementType);
                    dynamic mapper = _resolver.GetService(mapperType);
                    var list = mapper.Map(persistentObjectIds);
                    property.SetValue(instance, list, null);
                }
                else
                {
                    property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                }
                nav.MoveToParent();
            }
            return instance;
        }

        /// <summary>
        /// Saves the instance into XML document using attribute information
        /// </summary>
        /// <param name="instance">the instance of T to be saved</param>
        public void SaveObject(T instance)
        {
            var innerXml = CreateXmlForObject(instance);

            // Create lock to ensure no concurrency issues
            lock (LockObject)
            {
                var doc = _storageDocument.Get();

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

                _storageDocument.Save(doc);
            } // End lock
        }

        private static string CreateXmlForObject(T instance)
        {
            var objectStringBuilder = new StringBuilder();

            var settings = new XmlWriterSettings() { OmitXmlDeclaration = true };

            using (var writer = XmlWriter.Create(objectStringBuilder, settings))
            {
                writer.WriteStartElement(Constants.DataItemName);

                foreach (var property in typeof(T).GetValidProperties())
                {
                    var attributeName = property.GetPropertyName();

                    if (attributeName == PersistentObject.Identifier)
                    {
                        writer.WriteElementString(attributeName, property.GetValue(instance, null).ToString());
                    }
                    else
                    {
                        writer.WriteStartElement(attributeName);
                        if (property.PropertyType.IsAPersistentObject())
                        {
                            var persistentObject = property.GetValue(instance, null) as PersistentObject;
                            if (persistentObject != null) writer.WriteCData(persistentObject.Id.ToString());
                        }
                        else if (property.PropertyType.IsAPersistentObjectList())
                        {
                            var persistentObjectList = property.GetValue(instance, null) as IEnumerable<PersistentObject>;
                            if (persistentObjectList != null)
                            {
                                var flattenedList = string.Join<PersistentObject>(",", persistentObjectList);
                                writer.WriteCData(flattenedList);
                            }
                        }
                        else
                        {
                            writer.WriteCData(property.GetValue(instance, null).ToString());
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            return objectStringBuilder.ToString();
        }

        /// <summary>
        /// Delete an object from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the object</param>
        public void DeleteObject(Guid id)
        {
            // Create lock to ensure no concurrency issues
            lock (LockObject)
            {
                var doc = _storageDocument.Get();

                var objectNode = doc.SelectExistingNode(id);

                if (objectNode == null) return;
                if (doc.DocumentElement != null) doc.DocumentElement.RemoveChild(objectNode);
                _storageDocument.Save(doc);
            } // End lock
        }
    }
}
