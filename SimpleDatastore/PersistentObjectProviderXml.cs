using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;

namespace SimpleDatastore
{
    public class PersistentObjectProviderXml<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolverXml<T> _resolver;
        private readonly IDocumentProviderXml<T> _documentProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _persistChildren;

        public PersistentObjectProviderXml(IItemResolverXml<T> resolver,
            IDocumentProviderXml<T> documentProvider,
            IServiceProvider serviceProvider,
            IOptions<SimpleDatastoreOptions> options)
        {
            _resolver = resolver;
            _documentProvider = documentProvider;
            _serviceProvider = serviceProvider;
            _persistChildren = options.Value.PersistChildren;
        }

        ///<inheritdoc/>
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            var elements = doc.Descendants(PersistentObject.DataItemName);

            var tasks = elements.Select(element => _resolver.GetItemFromNodeAsync(element)).ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }

        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            var elements = _documentProvider.GetDocument().Descendants(PersistentObject.DataItemName);

            return elements.AsParallel().Select(element => _resolver.GetItemFromNode(element)).ToList();
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            var element = doc.GetElementById(id);

            return element == null ? null : await _resolver.GetItemFromNodeAsync(element).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public T GetObject(Guid id)
        {
            var element = _documentProvider.GetDocument().GetElementById(id);
            return element == null ? null : _resolver.GetItemFromNode(element);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = BuildXml(instance);

            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            var existingElement = doc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }

            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            var element = BuildXml(instance);

            var doc = _documentProvider.GetDocument();

            var existingElement = doc.GetElementById(instance.Id);

            if (existingElement != null)
            {
                existingElement.ReplaceWith(element);
            }
            else
            {
                doc.Root?.Add(element);
            }

            _documentProvider.SaveDocument(doc);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            doc.GetElementById(id)?.Remove();

            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void DeleteObject(Guid id)
        {
            var doc = _documentProvider.GetDocument();

            doc.GetElementById(id)?.Remove();

            _documentProvider.SaveDocument(doc);
        }

        internal XElement BuildXml(T instance)
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
                    if (_persistChildren)
                    {
                        var repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
                        dynamic repository = _serviceProvider.GetService(repositoryType);
                        repository.Save((dynamic) persistentObject);
                    }

                    element.Add(new XElement(attributeName, persistentObject.Id.ToString()));
                    continue;
                }

                if (property.PropertyType.IsAPersistentObjectEnumerable() &&
                    value is IEnumerable<PersistentObject> persistentObjectEnumerable)
                {
                    var list = persistentObjectEnumerable.ToList();

                    if (_persistChildren)
                    {
                        var elementType = property.PropertyType.GetGenericArguments()[0];
                        var repositoryType = typeof(IRepository<>).MakeGenericType(elementType);
                        dynamic repository = _serviceProvider.GetService(repositoryType);

                        foreach (var item in list)
                        {
                            repository.Save((dynamic) item);
                        }
                    }

                    var flattenedEnumerable = string.Join(",", list);
                    element.Add(new XElement(attributeName, flattenedEnumerable));
                    continue;
                }

                element.Add(new XElement(attributeName, new XCData(property.GetValue(instance, null).ToString())));
            }

            return element;
        }
    }
}