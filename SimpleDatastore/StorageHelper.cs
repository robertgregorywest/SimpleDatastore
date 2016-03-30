using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    internal class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        // ReSharper disable once StaticFieldInGenericType, lock is intended to be per type
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
            lock (LockObject)
            {
                var doc = _storageDocument.Get();
                var nav = doc.CreateNavigator();

                IList<T> collection = new List<T>();

                if (!nav.MoveToFirstChild()) return collection;

                var iterator = nav.Select(Constants.DataItemName);
                var xmlResolver = new XmlResolver<T>(_resolver);

                while (iterator.MoveNext())
                {
                    var navCurrent = iterator.Current;
                    var item = xmlResolver.GetItemFromNode(navCurrent);
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
                var doc = _storageDocument.Get();
                var nav = doc.CreateNavigator();

                if (!nav.MoveToFirstChild()) return null;
                var navCurrent =
                    nav.SelectSingleNode(string.Format("{0}[{1} = \"{2}\"]", Constants.DataItemName,
                        PersistentObject.Identifier, id.ToString()));
                if (navCurrent == null) return null;

                var item = new XmlResolver<T>(_resolver).GetItemFromNode(navCurrent);

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
                var doc = _storageDocument.Get();

                var objectNode = doc.SelectExistingNode(id);

                if (objectNode == null) return;
                if (doc.DocumentElement != null) doc.DocumentElement.RemoveChild(objectNode);
                _storageDocument.Save(doc);
            }
        }
    }
}
