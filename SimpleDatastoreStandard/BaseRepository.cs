using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository<T> where T : PersistentObject
    {
        private readonly IStorageHelper<T> _storageHelper;
        private readonly IConfiguration _config;
        private readonly ICache _cache;

        private static string KeyForObject(Guid id) => string.Format("{0}.{1}", typeof(T).ToString(), id.ToString());
        private static string KeyForCollection() => typeof(T).ToString();

        public BaseRepository(IStorageHelper<T> storageHelper, IConfiguration config, ICache cache)
        {
            _storageHelper = storageHelper;
            _config = config;
            _cache = cache;
        }

        public T Load(Guid id)
        {
            var result = _config.EnableCaching ? _cache.Get(KeyForObject(id)) as T : null;

            if (result != null)
            {
                return result;
            }

            result = _storageHelper.GetObject(id);

            if (_config.EnableCaching)
            {
                _cache.Set(KeyForObject(result.Id), result, _config.CacheDuration);
            }

            return result;
        }

        public IList<T> LoadList()
        {
            var list = LoadListUnsorted().ToList();
            list.Sort();
            return list;
        }

        public IList<T> LoadListUnsorted()
        {
            var result = _config.EnableCaching ? _cache.Get(KeyForCollection()) as IList<T> : null;

            if (result != null)
            {
                return result;
            }

            result = _storageHelper.GetCollection();

            if (_config.EnableCaching)
            {
                _cache.Set(KeyForCollection(), result, _config.CacheDuration);
            }

            return result;
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
