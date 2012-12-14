using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Mvc;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository<T> where T : PersistentObject
    {
        private readonly IConfiguration _configuration;

        private IStorageAgent<T> _helper;
        internal IStorageAgent<T> Helper
        {
            get
            {
                if (_helper == null)
                    _helper = new StorageAgent<T>(_configuration, new StorageDocument<T>(_configuration));
                return _helper;
            }
            set
            {
                _helper = value;
            }
        }

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Load(Guid id)
        {
            T instance = GetObjectFromCache(id) ?? Helper.GetObject(id);
            CacheObject(instance);
            return instance;
        }

        public IList<T> LoadList()
        {
            List<T> list = LoadListUnsorted().ToList();
            list.Sort();
            return list;
        }

        public IList<T> LoadListUnsorted()
        {
            IList<T> result = GetCollectionFromCache() ?? Helper.GetCollection();
            CacheCollection(result);
            return result;
        }

        public void Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }
            Helper.SaveObject(instance);
            PurgeCacheItems();
        }

        public void Delete(Guid id)
        {
            Helper.DeleteObject(id);
            PurgeCacheItems();
        }

        private string CacheKeyForObject(Guid id)
        {
            return string.Format("{0}.{1}", typeof(T).ToString(), id.ToString());
        }

        private string CacheKeyForCollection()
        {
            return typeof(T).ToString();
        }

        private T GetObjectFromCache(Guid id)
        {
            if (_configuration.EnableCaching)
            {
                return _configuration.Cache.Get(CacheKeyForObject(id)) as T;
            }
            return null;
        }

        private void CacheObject(T instance)
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(CacheKeyForObject(instance.Id), instance, _configuration.CacheDuration);
            }
        }

        private IList<T> GetCollectionFromCache()
        {
            if (_configuration.EnableCaching)
            {
                return _configuration.Cache.Get(CacheKeyForCollection()) as IList<T>;
            }
            return null;
        }

        private void CacheCollection(IList<T> collection)
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(CacheKeyForCollection(), collection, _configuration.CacheDuration);
            }
        }

        private void PurgeCacheItems()
        {
            if (_configuration.EnableCaching)
            {
                _configuration.Cache.PurgeCacheItems(typeof(T).ToString());
            }
        }
    }
}
