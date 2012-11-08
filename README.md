SimpleDatastore
===============

A simple .NET persistence library which uses XML file storage.

To use simply create instances of the IRepository<> class and configure your IoC container to use the BaseRepository<> class to instantiate them.

All your objects need to inherit from PersistenceObject. Decorate any fields you want to persist with the System.Runtime.Serialization.DataMember attribute.

The project currently has a dependency on the DependencyResolver so is focussed on ASP.NET MVC.

An example configuration using StructureMap:

ObjectFactory.Initialize(x =>
{
    x.For<IConfiguration>().Use<DefaultConfiguration>();
    x.For(typeof(IRepository<>)).Use(typeof(BaseRepository<>));
});

The default configuration uses the ASP.NET cache and stores data files in App_Data.