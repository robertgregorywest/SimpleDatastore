# SimpleDatastore
[![NuGet](https://img.shields.io/nuget/v/SimpleDatastore.svg)](https://www.nuget.org/packages/SimpleDatastore/)
[![Build Status](https://dev.azure.com/robgwest/SimpleDatastore/_apis/build/status/robertgregorywest.SimpleDatastore?branchName=master)](https://dev.azure.com/robgwest/SimpleDatastore/_build/latest?definitionId=3&branchName=master)

A simple .NET persistence library which uses either XML or JSON file storage.

This project uses .NET standard 2.1 and the built-in 
dependency resolution framework so it is super easy to implement.

## What's New in 3.1
The `PersistentObject` base type now uses a generic parameter to
determine the type of the identifier, so you are not limited to 
using Guids. You can continue to use the library in the same way
using the `IRepository` interface which will assume Guid 
identifiers. If you do not use Guid identifiers then you should use the
`IReadRepository<T, TKey>` and `IWriteRepository<T, TKey>`
interfaces. The `TKey` type must be a struct.

## How To Use
All of the entities that you wish to persist need to inherit from `PersistentObject<TKey>` 
and attributes are used to determine which fields to persist. The library stores 
each class in its own storage document.

Simply register SimpleDatastore in your `Startup.cs` and 
then add a dependency of `IRepository<>` on your types (assuming
are implementing PersistentObject<Guid>):

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

## Non-Guid Identifiers
If you do not use Guid identifiers then you should use the 
`IReadRepository<T, TKey>` and `IWriteRepository<T, TKey>` 
interfaces. The interfaces are split along read/write lines
to better follow SOLID principles.

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