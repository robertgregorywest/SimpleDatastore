using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class PersistentObjectProviderJsonSimple<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IDocumentProvider<T, string> _provider;

        public PersistentObjectProviderJsonSimple(IDocumentProvider<T, string> provider)
        {
            _provider = provider;
        }
        
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var dictionary = JsonSerializer.Deserialize<List<T>>(doc);

            return dictionary;
        }

        public IList<T> GetCollection()
        {
            var doc = _provider.GetDocument();

            var dictionary = JsonSerializer.Deserialize<List<T>>(doc);

            return dictionary;
        }

        public async Task<T> GetObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var dictionary = JsonSerializer.Deserialize<List<T>>(doc);

            return dictionary.Find(o => o.Id == id);
        }

        public T GetObject(Guid id)
        {
            var doc = _provider.GetDocument();

            var dictionary = JsonSerializer.Deserialize<List<T>>(doc);

            return dictionary.Find(o => o.Id == id);
        }

        public async Task SaveObjectAsync(T instance)
        {
            var doc = await _provider.GetDocumentAsync();

            var collection = JsonSerializer.Deserialize<List<T>>(doc);

            collection.AddOrReplace(instance);

            await _provider.SaveDocumentAsync(JsonSerializer.Serialize(collection));
        }

        public void SaveObject(T instance)
        {
            var doc = _provider.GetDocument();

            var collection = JsonSerializer.Deserialize<List<T>>(doc);

            collection.AddOrReplace(instance);

            _provider.SaveDocument(JsonSerializer.Serialize(collection));
        }

        public async Task DeleteObjectAsync(Guid id)
        {
            var doc = await _provider.GetDocumentAsync();

            var collection = await JsonSerializer.DeserializeAsync<List<T>>(doc.CreateStream());

            collection.RemoveAll(o => o.Id == id);

            await _provider.SaveDocumentAsync(JsonSerializer.Serialize(collection));
        }

        public void DeleteObject(Guid id)
        {
            var doc = _provider.GetDocument();

            var collection = JsonSerializer.Deserialize<List<T>>(doc);

            collection.RemoveAll(o => o.Id == id);

            _provider.SaveDocument(JsonSerializer.Serialize(collection));
        }
    }
}