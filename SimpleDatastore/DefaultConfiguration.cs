using System.Web;
using System.Web.Mvc;

namespace SimpleDatastore
{
    public class DefaultConfiguration : Configuration
    {
        public DefaultConfiguration(IDependencyResolver dependencyResolver) : base (dependencyResolver, new WebCache())
        {
            this.CacheDuration = 60;
            this.EnableCaching = true;
            this.DatastoreLocation = HttpContext.Current.Server.MapPath("~/App_Data/");
        } 
    }
}
