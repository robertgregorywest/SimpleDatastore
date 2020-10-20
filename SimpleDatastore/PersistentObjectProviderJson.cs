using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;
using static SimpleDatastore.PersistentObjectConverterJson;

namespace SimpleDatastore
{
    public class PersistentObjectProviderJson<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IItemResolver<T, JsonElement> _resolver;
        private readonly IDocumentProvider<T, JsonDocument> _documentProvider;
        private readonly bool _persistChildren;
        private readonly Func<Type, object> _activator;
        private readonly Func<Type, dynamic> _repoProvider;
        private readonly JsonWriterOptions _writerOptions = new JsonWriterOptions { Indented = true };

        public PersistentObjectProviderJson(IItemResolver<T, JsonElement> resolver,
            IDocumentProvider<T, JsonDocument> documentProvider,
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
            using var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);

            if (!_persistChildren)
            {
                return doc.Deserialize<List<T>>();
            }
            
            var tasks = doc.RootElement.EnumerateArray()
                .Select(element => _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren))
                .ToList();

            return (await tasks.WhenAll().ConfigureAwait(false)).ToList();
        }

        ///<inheritdoc/>
        public IList<T> GetCollection()
        {
            using var doc = _documentProvider.GetDocument();
            
            if (!_persistChildren)
            {
                return doc.Deserialize<List<T>>();
            }
            
            return doc.RootElement.EnumerateArray()
                .AsParallel()
                .Select(element => _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren))
                .ToList();
        }

        ///<inheritdoc/>
        public async Task<T> GetObjectAsync(Guid id)
        {
            using var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);
            
            var element = doc.RootElement.EnumerateArray()
                .AsParallel()
                .FirstOrDefault(e => e.IsPersistentObjectMatchById(id));

            return element.ValueKind == JsonValueKind.Undefined
                ? null
                : await _resolver.GetItemFromNodeAsync(element, _activator, _repoProvider, _persistChildren)
                    .ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public T GetObject(Guid id)
        {
            using var doc = _documentProvider.GetDocument();

            var element = doc.RootElement.EnumerateArray()
                .AsParallel()
                .FirstOrDefault(e => e.IsPersistentObjectMatchById(id));

            return element.ValueKind == JsonValueKind.Undefined
                ? null
                : _resolver.GetItemFromNode(element, _activator, _repoProvider, _persistChildren);
        }

        ///<inheritdoc/>
        public async Task SaveObjectAsync(T instance)
        {
            using var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);
            var replacement = Write(instance, _repoProvider, _persistChildren);
            await UpdateAndSaveDocumentAsync(doc, element => !element.IsPersistentObjectMatchById(instance.Id), replacement);
        }

        ///<inheritdoc/>
        public void SaveObject(T instance)
        {
            using var doc = _documentProvider.GetDocument();
            var replacement = Write(instance, _repoProvider, _persistChildren);
            UpdateAndSaveDocument(doc, element => !element.IsPersistentObjectMatchById(instance.Id), replacement);
        }

        ///<inheritdoc/>
        public async Task DeleteObjectAsync(Guid id)
        {
            using var doc = await _documentProvider.GetDocumentAsync().ConfigureAwait(false);
            await UpdateAndSaveDocumentAsync(doc, element => !element.IsPersistentObjectMatchById(id));
        }

        ///<inheritdoc/>
        public void DeleteObject(Guid id)
        {
            using var doc = _documentProvider.GetDocument();
            UpdateAndSaveDocument(doc, element => !element.IsPersistentObjectMatchById(id));
        }

        private void UpdateAndSaveDocument(JsonDocument doc, Func<JsonElement, bool> predicate, JsonElement replacement = default)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, _writerOptions);
            
            writer.WriteStartArray();
            
            foreach (var element in doc.RootElement.EnumerateArray().Where(predicate))
            {
                element.WriteTo(writer);
            }

            if (replacement.ValueKind != JsonValueKind.Undefined)
            {
                replacement.WriteTo(writer);
            }
            
            writer.WriteEndArray();

            writer.Flush();
            stream.Position = 0;

            using var updatedDoc = JsonDocument.Parse(stream);
            _documentProvider.SaveDocument(updatedDoc);
        }
        
        private async Task UpdateAndSaveDocumentAsync(JsonDocument doc, Func<JsonElement, bool> predicate, JsonElement replacement = default)
        {
            await using var stream = new MemoryStream();
            await using var writer = new Utf8JsonWriter(stream, _writerOptions);
            
            writer.WriteStartArray();
            
            foreach (var element in doc.RootElement.EnumerateArray().Where(predicate))
            {
                element.WriteTo(writer);
            }

            if (replacement.ValueKind != JsonValueKind.Undefined)
            {
                replacement.WriteTo(writer);
            }
            
            writer.WriteEndArray();

            await writer.FlushAsync();
            stream.Position = 0;

            using var updatedDoc = await JsonDocument.ParseAsync(stream);
            await _documentProvider.SaveDocumentAsync(updatedDoc);
        }
    }
}