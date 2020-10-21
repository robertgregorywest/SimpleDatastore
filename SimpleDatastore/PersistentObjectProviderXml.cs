﻿using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using static SimpleDatastore.PersistentObjectConverterXml;

namespace SimpleDatastore
{
    public class PersistentObjectProviderXml<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolver<T, XElement> _resolver;
        private readonly IDocumentProvider<T, XDocument> _documentProvider;
        private readonly bool _persistChildren;
        private readonly Func<Type, object> _activator;
        private readonly Func<Type, dynamic> _repoProvider;

        public PersistentObjectProviderXml(IItemResolver<T, XElement> resolver,
            IDocumentProvider<T, XDocument> documentProvider,
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

            doc.AddOrUpdate(instance, element);

            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            var element = Write(instance, _repoProvider, _persistChildren);

            var doc = _documentProvider.GetDocument().AddOrUpdate(instance, element);

            _documentProvider.SaveDocument(doc);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);
            doc.RemoveById(id);
            await _documentProvider.SaveDocumentAsync(doc).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void DeleteObject(Guid id)
        {
            var doc = _documentProvider.GetDocument().RemoveById(id);
            _documentProvider.SaveDocument(doc);
        }
    }
}