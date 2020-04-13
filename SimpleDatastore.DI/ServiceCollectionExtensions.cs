using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimpleDatastore.Interfaces;
using System.IO;

// ReSharper disable once CheckNamespace
namespace SimpleDatastore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services)
        {
            return AddSimpleDatastore(services, new Configuration(60, true, Path.Combine(services.BuildServiceProvider().GetService<IHostingEnvironment>().ContentRootPath, "App_Data")));
        }

        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services, IConfiguration config)
        {
            return ConfigureSimpleDatastore(services, config);
        }

        private static IServiceCollection ConfigureSimpleDatastore(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddSingleton(typeof(IStorageHelper<>), typeof(StorageHelper<>));
            services.AddSingleton(typeof(IXmlDocumentProvider<>), typeof(XmlDocumentProvider<>));
            services.AddSingleton(typeof(IXmlResolver<>), typeof(XmlResolver<>));
            services.AddSingleton(config);
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();

            return services;
        }
    }
}
