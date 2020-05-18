using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SimpleDatastore.Interfaces
{
    internal interface IRepository
    {
        Task<object> LoadObjectAsync(Guid id);
        Task<object> LoadObjectCollectionByIdsAsync(string[] persistentObjectIds);
    }
    
    public interface IRepository<T> where T : PersistentObject
    {
        /// <summary>
        /// Get a single persistent object
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Persistent object</returns>
        [UsedImplicitly]
        Task<T> LoadAsync(Guid id);

        /// <summary>
        /// Get all persistent objects of generic type in the order they are stored in the storage document
        /// </summary>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        Task<IEnumerable<T>> LoadCollectionAsync();

        /// <summary>
        /// Get persistent objects based on provided identifiers
        /// </summary>
        /// <param name="persistentObjectIds">The array of persistent object identifiers to retrieve</param>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        Task<IEnumerable<T>> LoadCollectionByIdsAsync(IEnumerable<string> persistentObjectIds);
        
        /// <summary>
        /// Save a persistent object to the storage document
        /// </summary>
        /// <param name="instance">Instance to save</param>
        [UsedImplicitly]
        Task SaveAsync(T instance);
        
        /// <summary>
        /// Delete a persistent object from the storage document
        /// </summary>
        /// <param name="id">Identifier to delete</param>
        [UsedImplicitly]
        Task DeleteAsync(Guid id);
    }
}
