using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SimpleDatastore
{
    public class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        // Lock intended to be per type
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object LockObject = new object();

        private readonly IItemResolver<T> _resolver;
        private readonly IDocumentProvider<T> _provider;

        public StorageHelper(IItemResolver<T> resolver, IDocumentProvider<T> provider)
        {
            _resolver = resolver;
            _provider = provider;
        }

        public IEnumerable<T> GetCollection()
        {
            lock (LockObject)
            {
                var nav = _provider.GetDocument().CreateNavigator();

                if (!nav.MoveToFirstChild()) yield break;

                var iterator = nav.Select(Constants.DataItemName);

                while (iterator.MoveNext())
                {
                    var navCurrent = iterator.Current;
                    yield return _resolver.GetItemFromNode(navCurrent);
                }
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
                    doc.DocumentElement?.ReplaceChild(objectDocFrag, existingNode);
                }
                else
                {
                    doc.DocumentElement?.AppendChild(objectDocFrag);
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

                doc.DocumentElement?.RemoveChild(objectNode);

                _provider.SaveDocument(doc);
            }
        }

        private static string BuildXml(T instance)
        {
            var objectStringBuilder = new StringBuilder();

            var settings = new XmlWriterSettings() {OmitXmlDeclaration = true};

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
                            if (property.GetValue(instance, null) is PersistentObject persistentObject)
                                writer.WriteCData(persistentObject.Id.ToString());
                        }
                        else if (property.PropertyType.IsAPersistentObjectEnumerable())
                        {
                            if (property.GetValue(instance, null) is IEnumerable<PersistentObject> persistentObjectEnumerable)
                            {
                                var flattenedEnumerable = string.Join(",", persistentObjectEnumerable);
                                writer.WriteCData(flattenedEnumerable);
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