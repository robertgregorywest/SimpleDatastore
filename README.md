SimpleDatastore
===============

A simple .NET persistence library which uses XML file storage.

This project has now updated to use .NET standard and the built-in dependency resolution framework. The easiest way to use it in an ASP.NET Core app is with the additional  NuGet package SimpleDatastore.Extensions.Microsoft.DependencyInjection. Simply include the following in your ConfigureServices to use the default configuration:

services.AddSimpleDatastore();

Then to use simply create instances of the IRepository<> for your types. All your objects need to inherit from PersistentObject. Decorate any fields you want to persist with the System.Runtime.Serialization.DataMember attribute. The default configuration uses the ASP.NET IMemoryCache.

A full working example app is included in the solution.

[![Build Status](https://dev.azure.com/robgwest/SimpleDatastore/_apis/build/status/robertgregorywest.SimpleDatastore?branchName=master)](https://dev.azure.com/robgwest/SimpleDatastore/_build/latest?definitionId=3&branchName=master)
