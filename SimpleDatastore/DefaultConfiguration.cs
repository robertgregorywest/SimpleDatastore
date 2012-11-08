using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
