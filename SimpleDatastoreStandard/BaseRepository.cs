using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatastore.Extensions;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository<T> where T : PersistentObject
    {
        private readonly IStorageHelper<T> _storageHelper;
        private readonly IConfiguration _config;
        private readonly ICache _cache;

        private static string KeyForObject(Guid id)
        {
            return $"{typeof(T)}.{id.ToString()}";
        }

        private static string KeyForCollection()
        {
            return typeof(T).ToString();
        }

        public BaseRepository(IStorageHelper<T> storageHelper, IConfiguration config, ICache cache)
        {
            _storageHelper = storageHelper;
            _config = config;
            _cache = cache;
        }

        public T Load(Guid id)
        {
            return GetCacheItem(() => _storageHelper.GetObject(id), KeyForObject(id));
        }

        public IList<T> LoadList()
        {
            var list = LoadListUnsorted().ToList();
            list.Sort();
            return list;
        }

        public IList<T> LoadListUnsorted()
        {
            return GetCacheItem<IList<T>>(() => _storageHelper.GetCollection(), KeyForCollection());
        }

        public IList<T> LoadListByIds(string[] persistentObjectIds)
        {
            return persistentObjectIds.Select(id => Load(id.ToGuid())).Where(po => po != null).ToList();
        }

        public void Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            _storageHelper.SaveObject(instance);

            PurgeCacheItem(instance.Id);
        }

        public void Delete(Guid id)
        {
            _storageHelper.DeleteObject(id);
            PurgeCacheItem(id);
        }

        private TResult GetCacheItem<TResult>(Func<TResult> func, string cacheKey)
        {
            if (!_config.EnableCaching)
            {
                return func.Invoke();
            }

            object cacheItem = _cache.Get(cacheKey);

            if (cacheItem is TResult result)
            {
                return result;
            }

            result = func.Invoke();

            if (result != null)
            {
                _cache.Set(cacheKey, result, _config.CacheDuration);
            }

            return result;
        }

        private void PurgeCacheItem(Guid id)
        {
            if (_config.EnableCaching)
            {
                _cache.Remove(KeyForObject(id));
                _cache.Remove(KeyForCollection());
            }
        }
    }
}
