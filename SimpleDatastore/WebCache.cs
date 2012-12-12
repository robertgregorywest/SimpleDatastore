using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace SimpleDatastore
{
    public class WebCache : ICache
    {
        private Cache Cache
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
            List<string> itemsToRemove = new List<string>();

            IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string itemToRemove in itemsToRemove)
            {
                Cache.Remove(itemToRemove);
            }
        }
    }
}
