using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class Repository<T, TKey> : IReadRepository<T, TKey>, IWriteRepository<T, TKey> 
        where T : PersistentObject<TKey> where TKey : struct
    {
        private readonly IPersistentObjectProvider<T, TKey> _persistentObjectProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly bool _cachingIsDisabled;
        private readonly int _cacheDuration;
        
        private static readonly string KeyForType = typeof(T).ToString();

        private static string KeyForInstance(TKey id) => $"{KeyForType}.{id.ToString()}";

        public Repository(IPersistentObjectProvider<T, TKey> persistentObjectProvider,
            IMemoryCache memoryCache, IOptions<SimpleDatastoreOptions> options)
        {
            _persistentObjectProvider = persistentObjectProvider;
            _memoryCache = memoryCache;
            _cachingIsDisabled = !options.Value.EnableCaching;
            _cacheDuration = options.Value.CacheDuration;
        }

        ///<inheritdoc/>
        public async Task<T> LoadAsync(TKey id)
        {
            if (_cachingIsDisabled)
            {
                return await _persistentObjectProvider.GetObjectAsync(id).ConfigureAwait(false);
            }
            return await _memoryCache.GetOrCreateAsync(KeyForInstance(id),
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheDuration));
                    return await _persistentObjectProvider.GetObjectAsync(id);
                }).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public T Load(TKey id)
        {
            if (_cachingIsDisabled)
            {
                return _persistentObjectProvider.GetObject(id);
            }
            return _memoryCache.GetOrCreate(KeyForInstance(id),
                (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheDuration));
                    return _persistentObjectProvider.GetObject(id);
                });
        }

        ///<inheritdoc/>
        public async Task<IList<T>> LoadCollectionAsync()
        {
            if (_cachingIsDisabled)
            {
                return await _persistentObjectProvider.GetCollectionAsync().ConfigureAwait(false);
            }
            return await _memoryCache.GetOrCreateAsync(KeyForType,
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheDuration));
                    return await _persistentObjectProvider.GetCollectionAsync();
                }).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public IList<T> LoadCollection()
        {
            if (_cachingIsDisabled)
            {
                return _persistentObjectProvider.GetCollection();
            }
            return _memoryCache.GetOrCreate(KeyForType,
                 (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheDuration));
                    return _persistentObjectProvider.GetCollection();
                });
        }

        ///<inheritdoc/>
        public async Task<IList<T>> LoadCollectionByIdsAsync(IEnumerable<string> persistentObjectIds)
        {
            var items = new List<T>();
            foreach (var id in persistentObjectIds)
            {
                var item = await LoadAsync(id.GetKeyFromString<TKey>()).ConfigureAwait(false);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items;
        }
        
        ///<inheritdoc/>
        public IList<T> LoadCollectionByIds(IEnumerable<string> persistentObjectIds)
        {
            return persistentObjectIds.Select(id => Load(id.GetKeyFromString<TKey>())).Where(item => item != null).ToList();
        }

        ///<inheritdoc/>
        public async Task SaveAsync(T instance)
        {
            await _persistentObjectProvider.SaveObjectAsync(instance).ConfigureAwait(false);
            PurgeCache(instance.Id);
        }
        
        ///<inheritdoc/>
        public void Save(T instance)
        {
            _persistentObjectProvider.SaveObject(instance);
            PurgeCache(instance.Id);
        }

        ///<inheritdoc/>
        public async Task DeleteAsync(TKey id)
        {
            await _persistentObjectProvider.DeleteObjectAsync(id).ConfigureAwait(false);
            PurgeCache(id);
        }
        
        ///<inheritdoc/>
        public void Delete(TKey id)
        {
            _persistentObjectProvider.DeleteObject(id);
            PurgeCache(id);
        }

        private void PurgeCache(TKey id)
        {
            if (_cachingIsDisabled) return;
            
            _memoryCache.Remove(KeyForInstance(id));
            _memoryCache.Remove(KeyForType);
        }
    }
}
