namespace SimpleDatastore
{
    public interface ICache
    {
        object Get(string key);
        void CacheData(string cacheKey, object data, int cacheDuration);
        void PurgeCacheItems(string prefix);
    }
}
