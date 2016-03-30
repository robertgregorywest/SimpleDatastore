using System.Web.Mvc;
using Example.DependencyResolution;

namespace Example
{
    public class StructureMapConfig
    {
        public static void ConfigureContainer()
        {
            var container = IoC.Initialize();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}