using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SimpleDatastore.Interfaces
{
    internal interface IRepository
    {
        object LoadObject(Guid id);
        object LoadObjectCollectionByIds(string[] persistentObjectIds);
    }
    
    public interface IRepository<T> where T : PersistentObject
    {
        /// <summary>
        /// Get a single persistent object
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Persistent object</returns>
        [UsedImplicitly]
        T Load(Guid id);

        /// <summary>
        /// Get all persistent objects of generic type in the order they are stored in the storage document
        /// </summary>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        IEnumerable<T> LoadCollection();

        /// <summary>
        /// Get persistent objects based on provided identifiers
        /// </summary>
        /// <param name="persistentObjectIds">The array of persistent object identifiers to retrieve</param>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        IEnumerable<T> LoadCollectionByIds(IEnumerable<string> persistentObjectIds);
        
        /// <summary>
        /// Save a persistent object to the storage document
        /// </summary>
        /// <param name="instance">Instance to save</param>
        [UsedImplicitly]
        void Save(T instance);
        
        /// <summary>
        /// Delete a persistent object from the storage document
        /// </summary>
        /// <param name="id">Identifier to delete</param>
        [UsedImplicitly]
        void Delete(Guid id);
    }
}
