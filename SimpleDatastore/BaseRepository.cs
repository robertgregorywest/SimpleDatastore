using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository, IRepository<T> where T : PersistentObject
    {
        private readonly IStorageHelper<T> _storageHelper;
        private readonly SimpleDatastoreOptions _options;
        private readonly IMemoryCache _memoryCache;
        
        private readonly string _keyForCollection = typeof(T).ToString();

        private static string KeyForObject(Guid id) => $"{typeof(T)}.{id.ToString()}";

        public BaseRepository(IStorageHelper<T> storageHelper, IOptions<SimpleDatastoreOptions> options, IMemoryCache memoryCache)
        {
            _storageHelper = storageHelper;
            _options = options.Value;
            _memoryCache = memoryCache;
        }

        ///<inheritdoc/>
        public async Task<T> LoadAsync(Guid id)
        {
            return await _memoryCache.GetOrCreateAsync(KeyForObject(id),
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.CacheDuration));
                    return await _storageHelper.GetObjectAsync(id);
                });
        }

        async Task<object> IRepository.LoadObjectAsync(Guid id)
        {
            return await LoadAsync(id);
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<T>> LoadCollectionAsync()
        {
            return await _memoryCache.GetOrCreateAsync(_keyForCollection,
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_options.CacheDuration));
                    return await _storageHelper.GetCollectionAsync();
                });
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<T>> LoadCollectionByIdsAsync(IEnumerable<string> persistentObjectIds)
        {
            var items = new List<T>();
            foreach (var id in persistentObjectIds)
            {
                var item = await LoadAsync(id.ToGuid());
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        async Task<object> IRepository.LoadObjectCollectionByIdsAsync(string[] persistentObjectIds)
        {
            return await LoadCollectionByIdsAsync(persistentObjectIds);
        }

        ///<inheritdoc/>
        public async Task SaveAsync(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            await _storageHelper.SaveObjectAsync(instance);
            PurgeCache(instance.Id);
        }

        ///<inheritdoc/>
        public async Task DeleteAsync(Guid id)
        {
            await _storageHelper.DeleteObjectAsync(id);
            PurgeCache(id);
        }

        private void PurgeCache(Guid id)
        {
            if (!_options.EnableCaching) return;
            
            _memoryCache.Remove(KeyForObject(id));
            _memoryCache.Remove(_keyForCollection);
        }
    }
}
