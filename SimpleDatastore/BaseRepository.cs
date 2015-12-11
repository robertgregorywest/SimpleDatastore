using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository<T> where T : PersistentObject
    {
        private readonly IConfiguration _configuration;

        private IStorageHelper<T> _storageHelper;
        internal IStorageHelper<T> StorageHelper
        {
            get {
                return _storageHelper ??
                       (_storageHelper =
                           new StorageHelper<T>(_configuration.DependencyResolver,
                               new StorageDocument<T>(_configuration)));
            }
            set
            {
                _storageHelper = value;
            }
        }

        private ICacheHelper<T> _cacheHelper;
        internal ICacheHelper<T> CacheHelper
        {
            get { return _cacheHelper ?? (_cacheHelper = new CacheHelper<T>(_configuration)); }
            set
            {
                _cacheHelper = value;
            }
        }

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Load(Guid id)
        {
            var result = CacheHelper.GetObject(id);

            if (result != null) return result;
            result = StorageHelper.GetObject(id);
            CacheHelper.CacheObject(result);

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
            var result = CacheHelper.GetCollection();

            if (result != null) return result;
            result = StorageHelper.GetCollection();
            CacheHelper.CacheCollection(result);

            return result;
        }

        public void Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }
            StorageHelper.SaveObject(instance);
            CacheHelper.PurgeCacheItems();
        }

        public void Delete(Guid id)
        {
            StorageHelper.DeleteObject(id);
            CacheHelper.PurgeCacheItems();
        }
    }
}
