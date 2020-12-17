using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimpleDatastore.Interfaces;

namespace SimpleDatastore
{
    public class GuidRepository<T> : Repository<T, Guid>, IRepository<T> where T : PersistentObject<Guid>
    {
        public GuidRepository(IPersistentObjectProvider<T, Guid> persistentObjectProvider,
            IMemoryCache memoryCache,
            IOptions<SimpleDatastoreOptions> options) : base(persistentObjectProvider,
            memoryCache,
            options)
        {
        }
    }
}