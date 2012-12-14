using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Web;

namespace SimpleDatastore
{
    internal class StorageAgent<T> : IStorageAgent<T> where T : PersistentObject
    {
        private static object _lockObject = new Object();

        private readonly IConfiguration _configuration;
        private readonly IStorageDocument<T> _storageDocument;

        public StorageAgent(IConfiguration configuration, IStorageDocument<T> storageDocument)
        {
            _configuration = configuration;
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

            if (nav.MoveToFirstChild())
            {
                var iterator = nav.Select(Constants.DataItemName);

                while (iterator.MoveNext())
                {
                    XPathNavigator navCurrent = iterator.Current;
                    T item = GetItemFromNode(navCurrent);
                    collection.Add(item);
                }
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

            if (nav.MoveToFirstChild())
            {
                var navCurrent = nav.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, id.ToString()));
                if (navCurrent != null)
                {
                    T item = GetItemFromNode(navCurrent);
                    return item;
                }
            }
            return null;
        }

        private T GetItemFromNode(XPathNavigator nav)
        {
            // Using the resolver so that objects can have dependencies
            T instance = _configuration.DependencyResolver.GetService<T>();

            foreach (PropertyInfo property in typeof(T).GetValidProperties())
            {
                var attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute));

                if (nav.MoveToChild(attribute.Name, ""))
                {
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(instance, nav.Value, null);
                    }
                    else if (property.PropertyType == typeof(Guid))
                    {
                        Guid guid = new Guid(nav.Value);
                        property.SetValue(instance, guid, null);
                    }
                    else if (property.PropertyType.IsAPersistentObject())
                    {
                        var repositoryType = typeof(BaseRepository<>).MakeGenericType(property.PropertyType);
                        dynamic repository = _configuration.DependencyResolver.GetService(repositoryType);
                        var persistentObject = repository.Load(nav.Value.ToGuid());
                        property.SetValue(instance, persistentObject, null);
                    }
                    else if (property.PropertyType.IsAPersistentObjectList())
                    {
                        string[] persistentObjectIds = nav.Value.Split(',');
                        Type elementType = property.PropertyType.GetGenericArguments()[0];
                        Type mapperType = typeof(CollectionMapper<>).MakeGenericType(elementType);
                        dynamic mapper = _configuration.DependencyResolver.GetService(mapperType);
                        var list = mapper.Map(persistentObjectIds);
                        property.SetValue(instance, list, null);
                    }
                    else
                    {
                        property.SetValue(instance, Convert.ChangeType(nav.Value, property.PropertyType), null);
                    }
                    nav.MoveToParent();
                }
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
            lock (_lockObject)
            {
                var doc = _storageDocument.Get();

                var objectDocFrag = doc.CreateDocumentFragment();
                objectDocFrag.InnerXml = innerXml;

                var existingNode = doc.SelectExistingNode(instance.Id);

                if (existingNode != null)
                {
                    doc.DocumentElement.ReplaceChild(objectDocFrag, existingNode);
                }
                else
                {
                    doc.DocumentElement.AppendChild(objectDocFrag);
                }

                _storageDocument.Save(doc);
            } // End lock
        }

        private string CreateXmlForObject(T instance)
        {
            var objectStringBuilder = new StringBuilder();

            var settings = new XmlWriterSettings() { OmitXmlDeclaration = true };

            using (var writer = XmlWriter.Create(objectStringBuilder, settings))
            {
                writer.WriteStartElement(Constants.DataItemName);

                foreach (PropertyInfo property in typeof(T).GetValidProperties())
                {
                    var attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute));

                    if (attribute.Name == PersistentObject.Identifier)
                    {
                        writer.WriteElementString(attribute.Name, property.GetValue(instance, null).ToString());
                    }
                    else
                    {
                        writer.WriteStartElement(attribute.Name);
                        if (property.PropertyType.IsAPersistentObject())
                        {
                            var persistentObject = property.GetValue(instance, null) as PersistentObject;
                            writer.WriteCData(persistentObject.Id.ToString());
                        }
                        else if (property.PropertyType.IsAPersistentObjectList())
                        {
                            var persistentObjectList = property.GetValue(instance, null) as IEnumerable<PersistentObject>;
                            string flattenedList = string.Join<PersistentObject>(",", persistentObjectList);
                            writer.WriteCData(flattenedList);
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
            lock (_lockObject)
            {
                var doc = _storageDocument.Get();

                XmlNode objectNode = doc.SelectExistingNode(id);

                if (objectNode != null)
                {
                    doc.DocumentElement.RemoveChild(objectNode);
                    _storageDocument.Save(doc);
                }
            } // End lock
        }
    }
}
