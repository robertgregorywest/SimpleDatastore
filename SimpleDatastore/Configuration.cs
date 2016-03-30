using System.Web;
using System.Web.Mvc;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class Configuration : IConfiguration
    {
        public int CacheDuration { get; set; }
        public bool EnableCaching { get; set; }
        public string DatastoreLocation { get; set; }

        public ICache Cache { get; private set; }
        public IDependencyResolver DependencyResolver { get; private set; }

        public Configuration(IDependencyResolver dependencyResolver) :
            this(dependencyResolver, new WebCache(), 60, true, HttpContext.Current.Server.MapPath("~/App_Data/"))
        {}

        public Configuration(IDependencyResolver dependencyResolver, ICache cache, int cacheDuration, bool enableCaching, string datastoreLocation)
        {
            DependencyResolver = dependencyResolver;
            Cache = cache;
            CacheDuration = cacheDuration;
            EnableCaching = enableCaching;
            DatastoreLocation = datastoreLocation;
        }
    }
}
