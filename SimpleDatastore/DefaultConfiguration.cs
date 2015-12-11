using System.Web;
using System.Web.Mvc;

namespace SimpleDatastore
{
    public class DefaultConfiguration : Configuration
    {
        public DefaultConfiguration(IDependencyResolver dependencyResolver) : base (dependencyResolver, new WebCache())
        {
            CacheDuration = 60;
            EnableCaching = true;
            DatastoreLocation = HttpContext.Current.Server.MapPath("~/App_Data/");
        } 
    }
}
