using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleDatastore.Interfaces;
using System.IO;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace SimpleDatastore
{
    public static class ServiceCollectionExtensions
    {
        [UsedImplicitly]
        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services)
        {
            var contentRootPath = services.BuildServiceProvider().GetService<IHostingEnvironment>().ContentRootPath;
            var configuration = new Configuration(60, true, Path.Combine(contentRootPath, Constants.DataFolder));
            return ConfigureSimpleDatastore(services, configuration);
        }

        [UsedImplicitly]
        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services, IConfiguration configuration)
        {
            return ConfigureSimpleDatastore(services, configuration);
        }

        private static IServiceCollection ConfigureSimpleDatastore(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddSingleton(typeof(IStorageHelper<>), typeof(StorageHelper<>));
            services.AddSingleton(typeof(IDocumentProvider<>), typeof(DocumentProvider<>));
            services.AddSingleton(typeof(IItemResolver<>), typeof(XmlResolver<>));
            services.AddSingleton(configuration);
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();

            return services;
        }
    }
}
