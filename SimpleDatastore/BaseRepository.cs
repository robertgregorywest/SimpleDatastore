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

        private IStorageHelper<T> _storageHelper;
        internal IStorageHelper<T> StorageHelper
        {
            get
            {
                if (_storageHelper == null)
                    _storageHelper = new StorageHelper<T>(_configuration.DependencyResolver, new StorageDocument<T>(_configuration));
                return _storageHelper;
            }
            set
            {
                _storageHelper = value;
            }
        }

        private ICacheHelper<T> _cacheHelper;
        internal ICacheHelper<T> CacheHelper
        {
            get
            {
                if (_cacheHelper == null)
                    _cacheHelper = new CacheHelper<T>(_configuration);
                return _cacheHelper;
            }
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
            T result = CacheHelper.GetObject(id);

            if (result == null)
            {
                result = StorageHelper.GetObject(id);
                CacheHelper.CacheObject(result);
            }

            return result;
        }

        public IList<T> LoadList()
        {
            List<T> list = LoadListUnsorted().ToList();
            list.Sort();
            return list;
        }

        public IList<T> LoadListUnsorted()
        {
            IList<T> result = CacheHelper.GetCollection();

            if (result == null)
            {
                result = StorageHelper.GetCollection();
                CacheHelper.CacheCollection(result);
            }

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
