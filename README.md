# SimpleDatastore
[![NuGet](https://img.shields.io/nuget/v/SimpleDatastore.svg)](https://www.nuget.org/packages/SimpleDatastore/)
[![Build Status](https://dev.azure.com/robgwest/SimpleDatastore/_apis/build/status/robertgregorywest.SimpleDatastore?branchName=master)](https://dev.azure.com/robgwest/SimpleDatastore/_build/latest?definitionId=3&branchName=master)

A simple .NET persistence library which uses either XML or JSON file storage.

This project has now been updated to use .NET standard 2.1 and the built-in 
dependency resolution framework so it is super easy to implement. 
Simply register in your `Startup.cs` and 
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

All of the entities that you wish to persist need to inherit from `PersistentObject` 
and attributes are used to determine which fields to persist.

The library stores each class in its own storage document.

## Storage As XML or JSON
The project has been updated to provide the option of using JSON. 
The `System.Text.Json` library is used for this option. You can use JSON like this:

```csharp
public void ConfigureServices(IServiceCollection services)
{
	services.AddSimpleDatastoreWithJson();
}
```

For XML storage decorate any fields you want to persist with 
the `System.Runtime.Serialization.DataMember` attribute:

```csharp
[DataMember(Name = "name")]
public string Name { get; set; }
```

for JSON storage the `System.Text.Json.Serialization.JsonPropertyName` 
attribute is used:

```csharp
[JsonPropertyName("name")]
public string Name { get; set; }
```

Objects are instantiated using the service provider so they can have 
dependencies of their own. 

## Child Objects

You can have properties which are also persisted objects.
You can choose to store child persistent objects either embedded in the parent
or in the relevant storage document for that type (with the `Id` stored as the key
for retrieval on rehydration). Storage in separate documents can be useful if the
same object is shared across many parent entities as you can update it easily.
As this is a simple library, it is up to you to track changes and ensure data
integrity.

When using JSON and embedded child objects the standard `JsonSerializer` is
used. This will enable you to control the serialization with your own
convertors should you wish to.

## Configuration

The default configuration uses the ASP.NET `IMemoryCache` but you can implement your own 
version. The `SimpleDatastoreOptions` class provides the following settings:
* enabling caching using the `IMemoryCache` implementation (default is true)
* cache duration in minutes (default is 60 minutes)
* storage file location (default is /App_Data for old times' sake)
* storage mode, either XML or JSON

## Example Web App
A full working example ASP.NET Core 3.1 app is included in the solution.