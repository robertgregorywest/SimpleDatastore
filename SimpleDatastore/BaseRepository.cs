﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository, IRepository<T> where T : PersistentObject
    {
        private readonly IStorageHelper<T> _storageHelper;
        private readonly SimpleDatastoreOptions _options;
        private readonly ICache _cache;
        
        private readonly string _keyForCollection = typeof(T).ToString();

        private static string KeyForObject(Guid id) => $"{typeof(T)}.{id.ToString()}";

        public BaseRepository(IStorageHelper<T> storageHelper, IOptions<SimpleDatastoreOptions> options, ICache cache)
        {
            _storageHelper = storageHelper;
            _options = options.Value;
            _cache = cache;
        }

        ///<inheritdoc/>
        public T Load(Guid id)
        {
            return GetCacheItem(() => _storageHelper.GetObject(id), KeyForObject(id));
        }

        object IRepository.LoadObject(Guid id)
        {
            return Load(id);
        }

        ///<inheritdoc/>
        public IEnumerable<T> LoadCollection()
        {
            return GetCacheItem(() => _storageHelper.GetCollection(), _keyForCollection);
        }

        ///<inheritdoc/>
        public IEnumerable<T> LoadCollectionByIds(IEnumerable<string> persistentObjectIds)
        {
            return persistentObjectIds.Select(id => Load(id.ToGuid())).Where(p => p != null);
        }

        object IRepository.LoadObjectCollectionByIds(string[] persistentObjectIds)
        {
            return LoadCollectionByIds(persistentObjectIds);
        }

        ///<inheritdoc/>
        public void Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            _storageHelper.SaveObject(instance);

            PurgeCache(instance.Id);
        }

        ///<inheritdoc/>
        public void Delete(Guid id)
        {
            _storageHelper.DeleteObject(id);
            PurgeCache(id);
        }

        private TResult GetCacheItem<TResult>(Func<TResult> func, string cacheKey) where TResult : class
        {
            if (!_options.EnableCaching)
            {
                return func.Invoke();
            }

            var cacheItem = _cache.Get(cacheKey);

            if (cacheItem is TResult result)
            {
                return result;
            }

            result = func.Invoke();

            if (result != null)
            {
                _cache.Set(cacheKey, result, _options.CacheDuration);
            }

            return result;
        }

        private void PurgeCache(Guid id)
        {
            if (!_options.EnableCaching) return;
            
            _cache.Remove(KeyForObject(id));
            _cache.Remove(_keyForCollection);
        }
    }
}
