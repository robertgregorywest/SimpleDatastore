# SimpleDatastore
[![NuGet](https://img.shields.io/nuget/v/SimpleDatastore.svg)](https://www.nuget.org/packages/SimpleDatastore/)
[![Build Status](https://dev.azure.com/robgwest/SimpleDatastore/_apis/build/status/robertgregorywest.SimpleDatastore?branchName=master)](https://dev.azure.com/robgwest/SimpleDatastore/_build/latest?definitionId=3&branchName=master)

A simple .NET persistence library which uses XML file storage.

This project has now been updated to use .NET standard 2.1 and the built-in dependency resolution framework
so it is super easy to implement. Simply register in your `Startup.cs` and 
then add a dependency of `IRepository<>` on your types:

**Startup.cs**

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddSimpleDatastore();
}
```

**HomeController.cs**

```csharp
public class HomeController
{
	private readonly IRepository<Widget> _repository;

	public HomeController(IRepository<Widget> repository)
	{
		_repository = repository;
	}
}
```

All your objects need to inherit from `PersistentObject`. Decorate any fields you want to persist with 
the `System.Runtime.Serialization.DataMember` attribute:

```csharp
[DataMember(Name = "name")]
public string Name { get; set; }
```

Objects are instantiated using the 
service provider so they can have dependencies of their own. You can have properties which are 
also persisted objects.

A full working example ASP.NET Core 3.1 app is included in the solution.

## Configuration

The default configuration uses the ASP.NET `IMemoryCache` but you can implement your own 
version. The `SimpleDatastoreOptions` class provides the following settings:
* enabling caching using the `IMemoryCache` implementation (default is true)
* cache duration in minutes (default is 60 minutes)
* storage file location (default is /App_Data for old times' sake)

