using System.Web.Mvc;

namespace SimpleDatastore.Interfaces
{
    public interface IConfiguration
    {
        int CacheDuration { get; set; }
        bool EnableCaching { get; set; }
        string DatastoreLocation { get; set; }
        IDependencyResolver DependencyResolver { get; }
        ICache Cache { get; }
    }
}
