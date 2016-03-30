using System;
using System.Web;
using System.Web.Mvc;

namespace SimpleDatastore
{
    public class DefaultConfiguration : Configuration
    {
        public DefaultConfiguration(IDependencyResolver dependencyResolver) : base (dependencyResolver)
        {} 
    }
}
