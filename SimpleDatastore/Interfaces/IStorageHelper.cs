using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleDatastore.Interfaces
{
    public interface IStorageHelper<T> where T : PersistentObject
    {
        Task<IEnumerable<T>> GetCollectionAsync();
        Task<T> GetObjectAsync(Guid id);
        Task SaveObjectAsync(T instance);
        Task DeleteObjectAsync(Guid id);
    }
}
