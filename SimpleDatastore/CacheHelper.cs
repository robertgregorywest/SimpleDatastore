using System;
using System.Collections.Generic;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class CacheHelper<T> : ICacheHelper<T> where T : PersistentObject
    {
        private readonly IConfiguration _configuration;

        public CacheHelper(IConfiguration configuration)
        {
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
                return _configuration.Cache.Get(KeyForObject(id)) as T;
            }
            return null;
        }

        public IList<T> GetCollection()
        {
            if (_configuration.EnableCaching)
            {
                return _configuration.Cache.Get(KeyForCollection()) as IList<T>;
            }
            return null;
        }

        public void CacheObject(T instance)
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(KeyForObject(instance.Id), instance, _configuration.CacheDuration);
            }
        }

        public void CacheCollection(IList<T> collection)
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(KeyForCollection(), collection, _configuration.CacheDuration);
            }
        }

        public void PurgeCacheItems()
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.PurgeCacheItems(typeof(T).ToString());
            }
        }
    }
}
