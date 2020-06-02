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
        private readonly IPersistentObjectProvider<T> _persistentObjectProvider;
        private readonly IMemoryCache _memoryCache;

        private bool CachingIsDisabled { get; }
        private int CacheDuration { get; }
        private static string KeyForCollection { get; } = typeof(T).ToString();
        
        private static string KeyForObject(Guid id) => $"{typeof(T)}.{id.ToString()}";

        public BaseRepository(IPersistentObjectProvider<T> persistentObjectProvider,
            IMemoryCache memoryCache, IOptions<SimpleDatastoreOptions> options)
        {
            _persistentObjectProvider = persistentObjectProvider;
            _memoryCache = memoryCache;
            CachingIsDisabled = !options.Value.EnableCaching;
            CacheDuration = options.Value.CacheDuration;
        }

        ///<inheritdoc/>
        public async Task<T> LoadAsync(Guid id)
        {
            if (CachingIsDisabled)
            {
                return await _persistentObjectProvider.GetObjectAsync(id);
            }
            return await _memoryCache.GetOrCreateAsync(KeyForObject(id),
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheDuration));
                    return await _persistentObjectProvider.GetObjectAsync(id);
                });
        }

        async Task<object> IRepository.LoadObjectAsync(Guid id) => await LoadAsync(id);

        ///<inheritdoc/>
        public async Task<IList<T>> LoadCollectionAsync()
        {
            if (CachingIsDisabled)
            {
                return await _persistentObjectProvider.GetCollectionAsync();
            }
            return await _memoryCache.GetOrCreateAsync(KeyForCollection,
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheDuration));
                    return await _persistentObjectProvider.GetCollectionAsync();
                });
        }

        ///<inheritdoc/>
        public async Task<IList<T>> LoadCollectionByIdsAsync(IEnumerable<string> persistentObjectIds)
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

        async Task<object> IRepository.LoadObjectCollectionByIdsAsync(IEnumerable<string> persistentObjectIds) =>
            await LoadCollectionByIdsAsync(persistentObjectIds);

        ///<inheritdoc/>
        public async Task SaveAsync(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            await _persistentObjectProvider.SaveObjectAsync(instance);
            PurgeCache(instance.Id);
        }

        ///<inheritdoc/>
        public async Task DeleteAsync(Guid id)
        {
            await _persistentObjectProvider.DeleteObjectAsync(id);
            PurgeCache(id);
        }

        private void PurgeCache(Guid id)
        {
            if (CachingIsDisabled) return;
            
            _memoryCache.Remove(KeyForObject(id));
            _memoryCache.Remove(KeyForCollection);
        }
    }
}
