using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class Configuration : IConfiguration
    {
        public int CacheDuration { get; set; }
        public bool EnableCaching { get; set; }
        public string DatastoreLocation { get; set; }

        public Configuration(int cacheDuration, bool enableCaching, string datastoreLocation)
        {
            CacheDuration = cacheDuration;
            EnableCaching = enableCaching;
            DatastoreLocation = datastoreLocation;
        }
    }
}
