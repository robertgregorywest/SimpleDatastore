# SimpleDatastore
[![NuGet](https://img.shields.io/nuget/v/SimpleDatastore.svg)](https://www.nuget.org/packages/SimpleDatastore/)
[![Build Status](https://dev.azure.com/robgwest/SimpleDatastore/_apis/build/status/robertgregorywest.SimpleDatastore?branchName=master)](https://dev.azure.com/robgwest/SimpleDatastore/_build/latest?definitionId=3&branchName=master)

A simple .NET persistence library which uses either XML or JSON file storage.

## What's New in 3.0

This project has now been updated to use .NET standard 2.1 and the built-in 
dependency resolution framework so it is super easy to implement.

The library is now async enabled, with new suffix methods for 
backwards compatibility.

Support for JSON has been added, built on the new `System.Text.Json` framework.

You can now chose to store child `PersistentObject` entities embedded in the parent
using the new `PersistChildren` option set to `false`.

## How To Use

All of the entities that you wish to persist need to inherit from `PersistentObject` 
and attributes are used to determine which fields to persist. The library stores 
each class in its own storage document.

Simply register SimpleDatastore in your `Startup.cs` and 
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



## Storage As XML or JSON
The default storage type is XML, if you wish to use JSON you can do so
 like this:

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

and ensure that you apply `JsonIgnore` attribute to any fields you do not 
wish to be persisted:

```csharp
[JsonIgnore]
public string NotPersisted { get; set; }
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
convertors (applied using attributes) should you wish to.

## Configuration

The default configuration uses the ASP.NET `IMemoryCache` but you can implement your own 
version. The `SimpleDatastoreOptions` class provides the following settings:
* enabling caching using the `IMemoryCache` implementation (default is true)
* cache duration in minutes (default is 60 minutes)
* storage file location (default is /App_Data for old times' sake)
* storage mode, either XML or JSON

The standard options pattern is used, so you can configure it like this:

**Startup.cs**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSimpleDatastoreWithJson(
        options =>
        {
            options.EnableCaching = false;
            options.PersistChildren = false;
        });
}
```

## Example Web App
A full working example ASP.NET Core 3.1 app is included in the solution.