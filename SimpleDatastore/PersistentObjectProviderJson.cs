using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class PersistentObjectProviderJson<T> : IPersistentObjectProvider<T> where T : PersistentObject
    {
        private readonly IDocumentProviderJson<T> _provider;

        public PersistentObjectProviderJson(IDocumentProviderJson<T> provider)
        {
            _provider = provider;
        }
        
        public async Task<IList<T>> GetCollectionAsync()
        {
            var doc = await _provider.GetDocumentAsync().ConfigureAwait(false);

            var e = doc.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
            
            return JsonSerializer.Deserialize<IList<T>>(doc.ToString());
        }

        public IList<T> GetCollection()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetObjectAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public T GetObject(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SaveObjectAsync(T instance)
        {
            throw new NotImplementedException();
        }

        public void SaveObject(T instance)
        {
            using var doc = _provider.GetDocument();

            var dictionary = JsonSerializer.Deserialize<ConcurrentDictionary<Guid,T>>(doc.ToString());

            dictionary.AddOrUpdate(instance.Id, instance, (key, oldValue) => instance);

            var result = JsonSerializer.Serialize(dictionary);

            _provider.SaveDocument(JsonDocument.Parse(result));
        }

        public async Task DeleteObjectAsync(Guid id)
        {
            using var doc = await _provider.GetDocumentAsync();

            var updatedContent = doc.RootElement.EnumerateArray()
                .Where(el => el.GetProperty(PersistentObject.Identifier).GetGuid() != id)
                .Select(e => e.Clone()).ToString();

            using var updatedDoc = await JsonDocument.ParseAsync(updatedContent.CreateStream());
            
            await _provider.SaveDocumentAsync(updatedDoc);
        }

        public void DeleteObject(Guid id)
        {
            using var doc = _provider.GetDocument();

            var updatedContent = doc.RootElement.EnumerateArray()
                .Where(el => el.GetProperty(PersistentObject.Identifier).GetGuid() != id)
                .Select(e => e.Clone()).ToString();

            using var updatedDoc = JsonDocument.Parse(updatedContent);
            
            _provider.SaveDocument(updatedDoc);
        }
    }
}