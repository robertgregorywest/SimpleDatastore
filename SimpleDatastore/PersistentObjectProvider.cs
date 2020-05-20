using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleDatastore
{
    public class PersistentObjectProvider<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolver<T> _resolver;
        private readonly IDocumentProvider<T> _provider;

        public PersistentObjectProvider(IItemResolver<T> resolver, IDocumentProvider<T> provider)
        {
            _resolver = resolver;
            _provider = provider;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync();

            var list = new List<T>();

            var elements = doc.Descendants(Constants.DataItemName);

            foreach (var element in elements)
            {
                list.Add(await _resolver.GetItemFromNodeAsync(element));
            }

            return list;
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync();

            var element = GetElementById(doc, id);

            if (element == null) return null;

            var item = await _resolver.GetItemFromNodeAsync(element);

            return item;
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = BuildXml(instance);

            var doc = await _provider.GetDocumentAsync();
            
            var existingElement = GetElementById(doc, instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }

            await _provider.SaveDocumentAsync(doc);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync();

            var element = GetElementById(doc, id);

            if (element == null) return;

            element.Remove();

            await _provider.SaveDocumentAsync(doc);
        }

        internal static XElement GetElementById(XDocument doc, Guid id)
        {
            return doc
                .Descendants(Constants.DataItemName)
                .FirstOrDefault(el => el.Element(PersistentObject.Identifier)?.Value == id.ToString());
        }

        internal static XElement BuildXml(T instance)
        {
            var element = new XElement(Constants.DataItemName);
            
            foreach (var property in typeof(T).GetValidProperties())
            {
                var attributeName = property.GetPropertyName();

                if (attributeName == PersistentObject.Identifier)
                {
                    element.Add(new XElement(attributeName, property.GetValue(instance, null).ToString()));
                }
                else
                {
                    if (property.PropertyType.IsAPersistentObject())
                    {
                        if (property.GetValue(instance, null) is PersistentObject persistentObject)
                        {
                            element.Add(new XElement(attributeName, persistentObject.Id.ToString()));
                        }
                    }
                    else if (property.PropertyType.IsAPersistentObjectEnumerable())
                    {
                        if (property.GetValue(instance, null) is IEnumerable<PersistentObject> persistentObjectEnumerable)
                        {
                            var flattenedEnumerable = string.Join(",", persistentObjectEnumerable);
                            element.Add(new XElement(attributeName, flattenedEnumerable));
                        }
                    }
                    else
                    {
                        element.Add(new XElement(attributeName, new XCData(property.GetValue(instance, null).ToString())));
                    }
                }
            }
            return element;
        }
    }
}