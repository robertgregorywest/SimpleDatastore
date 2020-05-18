using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleDatastore
{
    public class StorageHelper<T> : IStorageHelper<T> where T : PersistentObject
    {
        private readonly IItemResolver<T> _resolver;
        private readonly IDocumentProvider<T> _provider;

        public StorageHelper(IItemResolver<T> resolver, IDocumentProvider<T> provider)
        {
            _resolver = resolver;
            _provider = provider;
        }

        public async Task<IEnumerable<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync();
            var nav = doc.CreateNavigator();
            
            if (!nav.MoveToFirstChild()) return null;

            var list = new List<T>();
            
            var iterator = nav.Select(Constants.DataItemName);

            while (iterator.MoveNext())
            {
                var navCurrent = iterator.Current;
                list.Add(await _resolver.GetItemFromNodeAsync(navCurrent));
            }

            return list;
        }

        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync();
            var nav = doc.CreateNavigator();

            if (!nav.MoveToFirstChild()) return null;
            var navCurrent = nav.SelectSingleNode($"{Constants.DataItemName}[{PersistentObject.Identifier} = \"{id.ToString()}\"]");
            if (navCurrent == null) return null;

            var item = await _resolver.GetItemFromNodeAsync(navCurrent);

            return item;
        }

        public async Task SaveObjectAsync(T instance)
        {
            var innerXml = BuildXml(instance);

            var doc = await _provider.GetDocumentAsync();

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

            await _provider.SaveDocumentAsync(doc);
        }

        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync();

            var objectNode = doc.SelectExistingNode(id);

            if (objectNode == null) return;

            doc.DocumentElement?.RemoveChild(objectNode);

            await _provider.SaveDocumentAsync(doc);
        }

        private static string BuildXml(T instance)
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