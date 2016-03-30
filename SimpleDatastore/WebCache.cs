using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class WebCache : ICache
    {
        private static Cache Cache
        {
            get { return HttpRuntime.Cache; }
        }

        public object Get(string key)
        {
            return Cache.Get(key);
        }

        public void CacheData(string cacheKey, object data, int cacheDuration)
        {
            if (data != null)
            {
                Cache.Insert(cacheKey, data, null, DateTime.Now.AddMinutes(cacheDuration), TimeSpan.Zero);
            }
        }

        public void PurgeCacheItems(string prefix)
        {
            prefix = prefix.ToLower();
            var itemsToRemove = new List<string>();

            var enumerator = Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                Cache.Remove(itemToRemove);
            }
        }
    }
}
