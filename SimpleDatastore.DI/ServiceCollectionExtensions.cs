﻿using Microsoft.Extensions.DependencyInjection;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleDatastore(this IServiceCollection services, IConfiguration config)
            => ConfigureSimpleDatastore(services, config);

        private static IServiceCollection ConfigureSimpleDatastore(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddSingleton(typeof(IStorageHelper<>), typeof(StorageHelper<>));

            services.AddSingleton(typeof(ICacheHelper<>), typeof(CacheHelper<>));

            services.AddSingleton(typeof(IStorageDocument<>), typeof(StorageDocument<>));

            services.AddSingleton(typeof(IXmlResolver<>), typeof(XmlResolver<>));

            services.AddSingleton(config);

            services.AddSingleton<ICache, MemoryCache>();

            return services;
        }
    }
}
