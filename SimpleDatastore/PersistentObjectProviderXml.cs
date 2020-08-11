using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using static SimpleDatastore.XmlSerializer;

namespace SimpleDatastore
{
    public class PersistentObjectProviderXml<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolverXml<T> _resolver;
        private readonly IDocumentProviderXml<T> _documentProvider;
        private readonly bool _persistChildren;
        private readonly Func<Type, object> _activator;
        private readonly Func<Type, dynamic> _repoProvider;

        public PersistentObjectProviderXml(IItemResolverXml<T> resolver,
            IDocumentProviderXml<T> documentProvider,
            IServiceProvider serviceProvider,
            IOptions<SimpleDatastoreOptions> options)
        {
            _resolver = resolver;
            _documentProvider = documentProvider;
            _persistChildren = options.Value.PersistChildren;
            _activator = t => ActivatorUtilities.CreateInstance(serviceProvider, t);
            _repoProvider = t => serviceProvider.GetService(t);
        }

        ///<inheritdoc/>
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            if (doc.Root == null) return new List<T>();
            
            var elements = doc.Root.Elements(PersistentObject.DataItemName);

            var tasks = elements.Select(element =>
                _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren)).ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }

        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            var xElement = _documentProvider.GetDocument().Root;
            
            if (xElement == null) return new List<T>();
            
            var elements = xElement.Elements(PersistentObject.DataItemName);

            return elements.AsParallel().Select(element =>
                _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren)).ToList();
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            var element = doc.GetElementById(id);

            return element == null
                ? null
                : await _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren)
                    .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public T GetObject(Guid id)
        {
            var element = _documentProvider.GetDocument().GetElementById(id);
            return element == null
                ? null
                : _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = Write(instance, _repoProvider, _persistChildren);

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
            var element = Write(instance, _repoProvider, _persistChildren);

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
    }
}