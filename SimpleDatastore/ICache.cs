using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SimpleDatastore
{
    public interface ICache
    {
        object Get(string key);
        void CacheData(string cacheKey, object data, int cacheDuration);
        void PurgeCacheItems(string prefix);
    }
}
