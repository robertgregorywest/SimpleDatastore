using System;
using System.Collections.Generic;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class CacheHelper<T> : ICacheHelper<T> where T : PersistentObject
    {
        private readonly ICache _cache;
        private readonly IConfiguration _configuration;

        public CacheHelper(ICache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        private static string KeyForObject(Guid id)
        {
            return string.Format("{0}.{1}", typeof(T).ToString(), id.ToString());
        }

        private static string KeyForCollection()
        {
            return typeof(T).ToString();
        }

        public T GetObject(Guid id)
        {
            if (_configuration.EnableCaching)
            {
                return _cache.Get(KeyForObject(id)) as T;
            }
            return null;
        }

        public IList<T> GetCollection()
        {
            if (_configuration.EnableCaching)
            {
                return _cache.Get(KeyForCollection()) as IList<T>;
            }
            return null;
        }

        public void CacheObject(T instance)
        {
            if (_configuration.EnableCaching)
            {
                _cache.Set(KeyForObject(instance.Id), instance, _configuration.CacheDuration);
            }
        }

        public void CacheCollection(IList<T> collection)
        {
            if (_configuration.EnableCaching)
            {
                _cache.Set(KeyForCollection(), collection, _configuration.CacheDuration);
            }
        }

        public void PurgeCacheItems(Guid id)
        {
            if (_configuration.EnableCaching)
            {
                _cache.Remove(KeyForObject(id));
                _cache.Remove(KeyForCollection());
            }
        }
    }
}
