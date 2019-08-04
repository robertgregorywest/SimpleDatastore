﻿using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SimpleDatastore
{
    public class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        private static readonly object LockObject = new object();

        private readonly IXmlResolver<T> _resolver;
        private readonly IXmlDocumentProvider<T> _provider;

        public StorageHelper(IXmlResolver<T> resolver, IXmlDocumentProvider<T> provider)
        {
            _resolver = resolver;
            _provider = provider;
        }

        public IList<T> GetCollection()
        {
            lock (LockObject)
            {
                var doc = _provider.GetDocument();
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

        public T GetObject(Guid id)
        {
            lock (LockObject)
            {
                var doc = _provider.GetDocument();
                var nav = doc.CreateNavigator();

                if (!nav.MoveToFirstChild()) return null;
                var navCurrent = nav.SelectSingleNode($"{Constants.DataItemName}[{PersistentObject.Identifier} = \"{id.ToString()}\"]");
                if (navCurrent == null) return null;

                var item = _resolver.GetItemFromNode(navCurrent);

                return item;
            }
        }

        public void SaveObject(T instance)
        {
            var innerXml = BuildXml(instance);

            lock (LockObject)
            {
                var doc = _provider.GetDocument();

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

                _provider.SaveDocument(doc);
            }
        }

        public void DeleteObject(Guid id)
        {
            lock (LockObject)
            {
                var doc = _provider.GetDocument();

                var objectNode = doc.SelectExistingNode(id);

                if (objectNode == null) return;

                if (doc.DocumentElement != null) doc.DocumentElement.RemoveChild(objectNode);
                _provider.SaveDocument(doc);
            }
        }

        private string BuildXml(T instance)
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
    }
}
