using System.Web.Mvc;

namespace SimpleDatastore
{
    public class Configuration : IConfiguration
    {
        public int CacheDuration { get; set; }
        public bool EnableCaching { get; set; }
        public string DatastoreLocation { get; set; }

        public ICache Cache { get; private set; }
        public IDependencyResolver DependencyResolver { get; private set; }

        public Configuration(IDependencyResolver dependencyResolver, ICache cache)
        {
            DependencyResolver = dependencyResolver;
            Cache = cache;
        }
    }
}
