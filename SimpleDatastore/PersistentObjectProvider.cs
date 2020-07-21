using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Nito.AsyncEx;

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
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var xDoc = await ParseDocAsync(doc).ConfigureAwait(false);

            var elements = xDoc.Descendants(Constants.DataItemName);
            
            var tasks = elements.Select(element => _resolver.GetItemFromNodeAsync(element)).ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }
        
        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            var elements = XDocument.Parse(_provider.GetDocument()).Descendants(Constants.DataItemName);

            return elements.AsParallel().Select(element => _resolver.GetItemFromNode(element)).ToList();
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);
            
            var xDoc = await ParseDocAsync(doc).ConfigureAwait(false);

            var element = xDoc.GetElementById(id);

            return element == null ? null : await _resolver.GetItemFromNodeAsync(element).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public T GetObject(Guid id)
        {
            var element = XDocument.Parse(_provider.GetDocument()).GetElementById(id);
            return element == null ? null : _resolver.GetItemFromNode(element);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = BuildXml(instance);

            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var xDoc = await ParseDocAsync(doc).ConfigureAwait(false);
            
            var existingElement = xDoc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                xDoc.Root?.Add(element);
            }

            await _provider.SaveDocumentAsync(xDoc.ToString(SaveOptions.None)).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            var element = BuildXml(instance);
            
            var xDoc = XDocument.Parse(_provider.GetDocument());
            
            var existingElement = xDoc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                xDoc.Root?.Add(element);
            }

            _provider.SaveDocument(xDoc.ToString(SaveOptions.None));
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var xDoc = await ParseDocAsync(doc).ConfigureAwait(false);

            xDoc.GetElementById(id)?.Remove();
            
            await _provider.SaveDocumentAsync(xDoc.ToString(SaveOptions.None)).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public void DeleteObject(Guid id)
        {
            var xDoc = XDocument.Parse(_provider.GetDocument());

            xDoc.GetElementById(id)?.Remove();

            _provider.SaveDocument(xDoc.ToString(SaveOptions.None));
        }
        
        internal static XElement BuildXml(T instance)
        {
            var element = new XElement(Constants.DataItemName);
            
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
        
        private static async Task<XDocument> ParseDocAsync(string doc)
        {
            using TextReader tr = new StringReader(doc);
            return await XDocument.LoadAsync(tr, LoadOptions.None, CancellationToken.None).ConfigureAwait(false);
        }
    }
}