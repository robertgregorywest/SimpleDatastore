using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
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
        private readonly TimeSpan _timeSpan;
        
        private static readonly string KeyForType = typeof(T).ToString();

        private static string KeyForInstance(TKey id) => $"{KeyForType}.{id.ToString()}";

        public Repository(IPersistentObjectProvider<T, TKey> persistentObjectProvider,
            IMemoryCache memoryCache, IOptions<SimpleDatastoreOptions> options)
        {
            _persistentObjectProvider = persistentObjectProvider;
            _memoryCache = memoryCache;
            _cachingIsDisabled = !options.Value.EnableCaching;
            _timeSpan = TimeSpan.FromMinutes(options.Value.CacheDuration);
        }

        ///<inheritdoc/>
        public async Task<T> LoadAsync(TKey id)
        {
            var option = await LoadOptionAsync(id);
            return option.Match(t => t, () => default);
        }
        
        ///<inheritdoc/>
        public async Task<Option<T>> LoadOptionAsync(TKey id)
        {
            if (_cachingIsDisabled)
            {
                var option = await _persistentObjectProvider.GetObjectAsync(id).ConfigureAwait(false);
                return option.Match((t => t), (T) null);
            }
            
            return await _memoryCache.GetOrCreateAsync(KeyForInstance(id),
                async (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(_timeSpan);
                    
                    var option = await _persistentObjectProvider.GetObjectAsync(id);
                    return option.Match((t => t), (T) default);
                }).ConfigureAwait(false);
        }
        
        ///<inheritdoc/>
        public T Load(TKey id) => LoadOption(id).Match((t => t), () => default);

        ///<inheritdoc/>
        public Option<T> LoadOption(TKey id)
        {
            if (_cachingIsDisabled)
            {
                return _persistentObjectProvider.GetObject(id);
            }
            
            return _memoryCache.GetOrCreate(KeyForInstance(id),
                (cacheEntry) =>
                {
                    cacheEntry.SetAbsoluteExpiration(_timeSpan);
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
                    cacheEntry.SetAbsoluteExpiration(_timeSpan);
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
                    cacheEntry.SetAbsoluteExpiration(_timeSpan);
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
