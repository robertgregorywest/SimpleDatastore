using StructureMap;
using SimpleDatastore;
using System.Web.Mvc;
using SimpleDatastore.Interfaces;

namespace Example.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            var container = new Container(x =>
            {
                x.For<IConfiguration>().Use<DefaultConfiguration>();
                x.For(typeof(IRepository<>)).Use(typeof(BaseRepository<>));
                x.For<IDependencyResolver>().Use(() => DependencyResolver.Current);
            });

            return container;
        }
    }
}