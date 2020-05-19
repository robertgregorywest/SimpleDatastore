using System;
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
        {
            if (options != null)
            {
                services.Configure(options);
            }

            services.AddSingleton(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddSingleton(typeof(IStorageHelper<>), typeof(StorageHelper<>));
            services.AddSingleton(typeof(IDocumentProvider<>), typeof(DocumentProvider<>));
            services.AddSingleton(typeof(IItemResolver<>), typeof(ItemResolver<>));
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddMemoryCache();

            return services;
        }
    }
}
