using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using static SimpleDatastore.PersistentObjectConverterXml;

namespace SimpleDatastore
{
    public class PersistentObjectProviderXml<T, TKey> : IPersistentObjectProvider<T, TKey> where T : PersistentObject<TKey> where TKey : struct
    {
        private readonly IItemResolver<T, TKey, XElement> _resolver;
        private readonly IDocumentProvider<T, TKey, XDocument> _documentProvider;
        private readonly bool _persistChildren;
        private readonly Func<Type, object> _activator;
        private readonly Func<Type, dynamic> _repoProvider;

        public PersistentObjectProviderXml(IItemResolver<T, TKey, XElement> resolver,
            IDocumentProvider<T, TKey,XDocument> documentProvider,
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

            var tasks = doc.Root?.Elements(PersistentObject.DataItemName)
                .Select(element => _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren))
                .ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }

        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            return _documentProvider.GetDocument().Root?.Elements(PersistentObject.DataItemName)
                .AsParallel()
                .Select(element => _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren))
                .ToList();
        }

        ///<inheritdoc/>
        public async Task<Option<T>> GetObjectAsync(TKey id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            var element = doc.GetElementById(id);

            return element == null
                ? Option<T>.None
                : await _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren)
                    .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public Option<T> GetObject(TKey id)
        {
            var element = _documentProvider.GetDocument().GetElementById(id);
            return element == null
                ? Option<T>.None
                : _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            var element = Write<TKey>(instance, _repoProvider, _persistChildren);

            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            doc.AddOrUpdate<T,TKey>(instance, element);

            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            var element = Write<TKey>(instance, _repoProvider, _persistChildren);

            var doc = _documentProvider.GetDocument().AddOrUpdate<T,TKey>(instance, element);

            _documentProvider.SaveDocument(doc);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(TKey id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);
            doc.RemoveById(id);
            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void DeleteObject(TKey id)
        {
            var doc = _documentProvider.GetDocument().RemoveById(id);
            _documentProvider.SaveDocument(doc);
        }
    }
}