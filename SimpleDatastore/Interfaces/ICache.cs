namespace SimpleDatastore.Interfaces
{
    public interface ICache
    {
        object Get(string key);
        void Set(string cacheKey, object data, int cacheDuration);
        void Remove(string key);
    }
}
