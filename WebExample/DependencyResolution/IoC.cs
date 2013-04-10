using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap;
using SimpleDatastore;
using System.Web.Mvc;

namespace WebExample.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<IConfiguration>().Use<DefaultConfiguration>();
                x.For(typeof(IRepository<>)).Use(typeof(BaseRepository<>));
                x.For<IDependencyResolver>().Use(() => DependencyResolver.Current);
            });
            return ObjectFactory.Container;
        }
    }
}