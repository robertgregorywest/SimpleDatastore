using StructureMap;
using System.Web.Mvc;
using Example.DependencyResolution;

namespace Example.App_Start
{
    public class StructureMapConfig
    {
        public static void ConfigureContainer()
        {
            IContainer container = IoC.Initialize();
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
    }
}