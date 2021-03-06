﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Legacy interface for <see cref="PersistentObject{Guid}"/> standard implementation
    /// </summary>
    /// <typeparam name="T">PersistentObject type to work with</typeparam>
    public interface IRepository<T> where T : PersistentObject<Guid> 
    {
        /// <summary>
        /// Get a single persistent object
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Persistent object</returns>
        [UsedImplicitly]
        Task<T> LoadAsync(Guid id);
        
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
        Task<IList<T>> LoadCollectionAsync();
        
        /// <summary>
        /// Get all persistent objects of generic type in the order they are stored in the storage document
        /// </summary>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        IList<T> LoadCollection();
        
        /// <summary>
        /// Save a persistent object to the storage document
        /// </summary>
        /// <param name="instance">Instance to save</param>
        [UsedImplicitly]
        Task SaveAsync(T instance);

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
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Delete a persistent object from the storage document
        /// </summary>
        /// <param name="id">Identifier to delete</param>
        [UsedImplicitly]
        void Delete(Guid id);
    }
}
