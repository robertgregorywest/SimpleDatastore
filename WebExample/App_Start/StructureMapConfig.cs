using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap;
using System.Web.Mvc;
using WebExample.DependencyResolution;

namespace WebExample.App_Start
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