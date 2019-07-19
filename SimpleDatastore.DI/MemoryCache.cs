using System;
using SimpleDatastore.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleDatastore
{
    public class MemoryCache : ICache
    {
        private readonly IMemoryCache _cache;

        public MemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Set(string cacheKey, object data, int cacheDuration)
        {
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(cacheDuration));
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
