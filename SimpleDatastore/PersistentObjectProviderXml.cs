using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class PersistentObjectProviderXml<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolverXml<T> _resolver;
        private readonly IDocumentProviderXml<T> _provider;

        public PersistentObjectProviderXml(IItemResolverXml<T> resolver, IDocumentProviderXml<T> provider)
        {
            _resolver = resolver;
            _provider = provider;
        }

        ///<inheritdoc/>
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var elements = doc.Descendants(PersistentObject.DataItemName);
            
            var tasks = elements.Select(element => _resolver.GetItemFromNodeAsync(element)).ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }
        
        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            var elements = _provider.GetDocument().Descendants(PersistentObject.DataItemName);
            
            return elements.AsParallel().Select(element => _resolver.GetItemFromNode(element)).ToList();
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);
            
            var element = doc.GetElementById(id);

            return element == null ? null : await _resolver.GetItemFromNodeAsync(element).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public T GetObject(Guid id)
        {
            var element = _provider.GetDocument().GetElementById(id);
            return element == null ? null : _resolver.GetItemFromNode(element);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = BuildXml(instance);

            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var existingElement = doc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }

            await _provider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            var element = BuildXml(instance);
            
            var doc = _provider.GetDocument();
            
            var existingElement = doc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }

            _provider.SaveDocument(doc);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            doc.GetElementById(id)?.Remove();
            
            await _provider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public void DeleteObject(Guid id)
        {
            var doc = _provider.GetDocument();

            doc.GetElementById(id)?.Remove();

            _provider.SaveDocument(doc);
        }
        
        internal static XElement BuildXml(T instance)
        {
            var element = new XElement(PersistentObject.DataItemName);
            
            foreach (var property in typeof(T).GetValidProperties())
            {
                var attributeName = property.GetPropertyName();
                var value = property.GetValue(instance, null);

                if (attributeName == PersistentObject.Identifier)
                {
                    element.Add(new XElement(attributeName, value.ToString()));
                    continue;
                }
                
                if (property.PropertyType.IsAPersistentObject() && value is PersistentObject persistentObject)
                {
                    element.Add(new XElement(attributeName, persistentObject.Id.ToString()));
                    continue;
                }
                
                if (property.PropertyType.IsAPersistentObjectEnumerable() && value is IEnumerable<PersistentObject> persistentObjectEnumerable)
                {
                    var flattenedEnumerable = string.Join(",", persistentObjectEnumerable);
                    element.Add(new XElement(attributeName, flattenedEnumerable));
                    continue;
                }
                
                element.Add(new XElement(attributeName, new XCData(property.GetValue(instance, null).ToString())));
            }
            return element;
        }
    }
}