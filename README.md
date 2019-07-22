SimpleDatastore
===============

A simple .NET persistence library which uses XML file storage.

This project has now updated to use .NET standard and the built in dependency resolution framework. The easiest way to use it in an ASP.NET Core app is with the additional  NuGet package SimpleDatastore.Extensions.Microsoft.DependencyInjection. Add an IHostingEnvironment to your Startup class and then  include the following in your ConfigureServices:

services.AddMemoryCache();
services.AddSimpleDatastore(new Configuration(60, true, System.IO.Path.Combine(HostingEnvironment.ContentRootPath, "App_Data")));

The to use simply create instances of the IRepository<> for your types. All your objects need to inherit from PersistenceObject. Decorate any fields you want to persist with the System.Runtime.Serialization.DataMember attribute. The default configuration uses the ASP.NET IMemoryCache.

A full working example app is included in the solution.