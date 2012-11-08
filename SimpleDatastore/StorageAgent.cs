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
                // Query the document for data items
                var iterator = nav.Select(Constants.DataItemName);

                // Iterate through the results
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
        /// Get an class instance from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the instance</param>
        /// <returns>The instance of T from the XML document</returns>
        public T GetObject(Guid id)
        {
            var doc = _storageDocument.Get();
            var nav = doc.CreateNavigator();

            if (nav.MoveToFirstChild())
            {
                // Query the document for data item
                var navCurrent = nav.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, id.ToString()));
                if (navCurrent != null)
                {
                    T item = GetItemFromNode(navCurrent);
                    return item;
                }
            }
            return null;
        }


        /// <summary>
        /// Get an instance of the supplied class from an XPathNavigator
        /// </summary>
        /// <param name="nav">The XPathNavigator to interrogate</param>
        /// <returns>An object of the specified type</returns>
        private T GetItemFromNode(XPathNavigator nav)
        {
            Type type = typeof(T);

            // Use the service locator to instatiate with dependencies
            T instance = _configuration.DependencyResolver.GetService<T>();

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // For each property declared in the type provided check if the property is decorated with the DataField attribute
                if (Attribute.IsDefined(property, typeof(DataMemberAttribute)))
                {
                    DataMemberAttribute attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute));

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
                        else if (typeof(PersistentObject).IsAssignableFrom(property.PropertyType))
                        {
                            var repositoryType = typeof(BaseRepository<>).MakeGenericType(property.PropertyType);
                            dynamic repository = _configuration.DependencyResolver.GetService(repositoryType);
                            var persistentObject = repository.Load(nav.Value.ToGuid());
                            property.SetValue(instance, persistentObject, null);
                        }
                        else if (TypeHelper.TypeIsAPersistentObjectList(property.PropertyType))
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
            }

            return instance;
        }

        /// <summary>
        /// Saves the instance into XML document using attribute information
        /// </summary>
        /// <param name="instance">the instance of T to be saved</param>
        public bool SaveObject(T instance)
        {
            bool success = false;

            PropertyInfo[] properties = typeof(T).GetProperties();

            // variable to hold object identifer
            Guid instanceId = Guid.Empty;

            // StringBuilder to hold object XML
            StringBuilder objectStringBuilder = new StringBuilder();

            // Ensure XML written with appropriate settings
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            // Populate StringBuilder with data from object
            using (XmlWriter writer = XmlWriter.Create(objectStringBuilder, settings))
            {
                writer.WriteStartElement(Constants.DataItemName);

                foreach (PropertyInfo property in properties)
                {
                    // for each property declared in the type provided check if the property is decorated with the DataMember attribute
                    if (Attribute.IsDefined(property, typeof(DataMemberAttribute)))
                    {
                        DataMemberAttribute attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(property, typeof(DataMemberAttribute));

                        if (attribute.Name == PersistentObject.Identifier)
                        {
                            instanceId = (Guid)property.GetValue(instance, null);
                            writer.WriteElementString(attribute.Name, property.GetValue(instance, null).ToString());
                            success = true;
                        }
                        else
                        {
                            writer.WriteStartElement(attribute.Name);
                            if (typeof(PersistentObject).IsAssignableFrom(property.PropertyType))
                            {
                                var persistentObject = property.GetValue(instance, null) as PersistentObject;
                                writer.WriteCData(persistentObject.Id.ToString());
                            }
                            else if (TypeHelper.TypeIsAPersistentObjectList(property.PropertyType))
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
                }
                writer.WriteEndElement();
            }

            if (success)
            {
                // Create lock to ensure no concurrency issues
                lock (_lockObject)
                {
                    var doc = _storageDocument.Get();

                    // Create document fragment for object XML from StringBuilder
                    XmlDocumentFragment objectDocFrag = doc.CreateDocumentFragment();
                    objectDocFrag.InnerXml = objectStringBuilder.ToString();

                    // Check if the object XML exists
                    XmlNode existingNode = doc.DocumentElement.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, instanceId.ToString()));

                    if (existingNode != null)
                    {
                        // Replace the node
                        doc.DocumentElement.ReplaceChild(objectDocFrag, existingNode);
                    }
                    else
                    {
                        // Add the node as the last element
                        doc.DocumentElement.AppendChild(objectDocFrag);
                    }

                    _storageDocument.Save(doc);
                } // End lock
            }

            return success;
        }

        /// <summary>
        /// Delete an object from an XML document by identifer
        /// </summary>
        /// <param name="id">The identifier for the object</param>
        /// <returns>Boolean indicating whether the object has been deleted</returns>
        public bool DeleteObject(Guid id)
        {
            bool ret = false;

            // Create lock to ensure no concurrency issues
            lock (_lockObject)
            {
                var doc = _storageDocument.Get();

                // Check if the object XML exists
                XmlNode objectNode = doc.DocumentElement.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName, PersistentObject.Identifier, id.ToString()));

                if (objectNode != null)
                {
                    // Remove the node
                    doc.DocumentElement.RemoveChild(objectNode);

                    _storageDocument.Save(doc);

                    ret = true;
                }
            } // End lock

            return ret;
        }
    }
}
