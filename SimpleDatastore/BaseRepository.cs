﻿using System;
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
            string cacheKey = string.Empty;

            if (_configuration.EnableCaching)
            {
                cacheKey = string.Format("{0}.{1}", typeof(T).ToString(), id.ToString());

                if (_configuration.Cache.Get(cacheKey) != null)
                {
                    return (T)_configuration.Cache.Get(cacheKey);
                }
            }

            T instance = Helper.GetObject(id);

            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(cacheKey, instance, _configuration.CacheDuration);
            }

            return instance;
        }

        public IList<T> LoadList()
        {
            List<T> list = LoadListUnSorted().ToList();
            list.Sort();
            return list;
        }

        public IList<T> LoadListUnSorted()
        {
            string cacheKey = string.Empty;

            if (_configuration.EnableCaching)
            {
                cacheKey = typeof(T).ToString();

                if (_configuration.Cache.Get(cacheKey) != null)
                {
                    return (List<T>)_configuration.Cache.Get(cacheKey);
                }
            }

            IList<T> list = Helper.GetCollection();

            if (_configuration.EnableCaching)
            {
                _configuration.Cache.CacheData(cacheKey, list, _configuration.CacheDuration);
            }

            return list;
        }

        public bool Save(T instance)
        {
            if (instance.Id == Guid.Empty)
            {
                instance.Id = Guid.NewGuid();
            }

            bool success = Helper.SaveObject(instance);
            if (success && _configuration.EnableCaching)
            {
                _configuration.Cache.PurgeCacheItems(typeof(T).ToString());
            }
            return success;
        }

        public bool Delete(Guid id)
        {
            bool success = Helper.DeleteObject(id);
            if (success && _configuration.EnableCaching)
            {
                _configuration.Cache.PurgeCacheItems(typeof(T).ToString());
            }
            return success;
        }
    }
}
