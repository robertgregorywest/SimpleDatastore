using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class BaseRepository<T> : IRepository<T> where T : PersistentObject
    {
        private readonly IStorageHelper<T> _storageHelper;
        private readonly ICacheHelper<T> _cacheHelper;

        public BaseRepository(IStorageHelper<T> storageHelper, ICacheHelper<T> cacheHelper)
        {
            _storageHelper = storageHelper;
            _cacheHelper = cacheHelper;
        }

        public T Load(Guid id)
        {
            var result = _cacheHelper.GetObject(id);

            if (result != null) return result;
            result = _storageHelper.GetObject(id);
            _cacheHelper.CacheObject(result);

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
            var result = _cacheHelper.GetCollection();

            if (result != null) return result;
            result = _storageHelper.GetCollection();
            _cacheHelper.CacheCollection(result);

            return result;
        }

        public void Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }
            _storageHelper.SaveObject(instance);
            _cacheHelper.PurgeCacheItems(instance.Id);
        }

        public void Delete(Guid id)
        {
            _storageHelper.DeleteObject(id);
            _cacheHelper.PurgeCacheItems(id);
        }
    }
}
