﻿using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SimpleDatastore.Interfaces;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace SimpleDatastore
{
    public static class ServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services, Action<SimpleDatastoreOptions> options = null) 
            => services.AddSimpleDatastore(true, options);
        
        [UsedImplicitly]
        public static IServiceCollection AddSimpleDatastoreWithXml(this IServiceCollection services, Action<SimpleDatastoreOptions> options = null) 
            => services.AddSimpleDatastore(true, options);
        
        [UsedImplicitly]
        public static IServiceCollection AddSimpleDatastoreWithJson(this IServiceCollection services, Action<SimpleDatastoreOptions> options = null) 
            => services.AddSimpleDatastore(false, options);

        private static IServiceCollection AddSimpleDatastore(this IServiceCollection services, bool useXml, Action<SimpleDatastoreOptions> options = null)
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.AddSingleton(typeof(IRepository<>), typeof(GuidRepository<>));
            services.AddSingleton(typeof(IReadRepository<,>), typeof(Repository<,>));
            services.AddSingleton(typeof(IWriteRepository<,>), typeof(Repository<,>));
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddMemoryCache();

            if (useXml)
            {
                services.AddSingleton(typeof(IDocumentProvider<,,>), typeof(DocumentProviderXml<,,>));
                services.AddSingleton(typeof(IItemResolver<,,>), typeof(ItemResolverXml<,,>));
                services.AddSingleton(typeof(IPersistentObjectProvider<,>), typeof(PersistentObjectProviderXml<,>));
            }
            else
            {
                services.AddSingleton(typeof(IDocumentProvider<,,>), typeof(DocumentProviderJson<,,>));
                services.AddSingleton(typeof(IItemResolver<,,>), typeof(ItemResolverJson<,,>));
                services.AddSingleton(typeof(IPersistentObjectProvider<,>), typeof(PersistentObjectProviderJson<,>));
            }

            return services;
        }
    }
}
