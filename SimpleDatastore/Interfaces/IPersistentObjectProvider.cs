using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Retrieves and creates XML for persistent objects
    /// </summary>
    /// <typeparam name="T">PersistentObject type to work with</typeparam>
    public interface IPersistentObjectProvider<T> where T : PersistentObject
    {
        /// <summary>
        /// Gets all persistent objects from the XML document
        /// </summary>
        /// <returns>An <see cref="IList{T}"/> of the persistent object type</returns>
        Task<IList<T>> GetCollectionAsync();
        
        /// <summary>
        /// Gets all persistent objects from the XML document
        /// </summary>
        /// <returns>An <see cref="IList{T}"/> of the persistent object type</returns>
        IList<T> GetCollection();
        
        /// <summary>
        /// Gets a single persistent object based on the identifier from the XML document
        /// </summary>
        /// <param name="id">The identifier of the persistent object to retrieve</param>
        /// <returns>Persistent object</returns>
        Task<T> GetObjectAsync(Guid id);
        
        /// <summary>
        /// Gets a single persistent object based on the identifier from the XML document
        /// </summary>
        /// <param name="id">The identifier of the persistent object to retrieve</param>
        /// <returns>Persistent object</returns>
        T GetObject(Guid id);
        
        /// <summary>
        /// Update the XML document with the persistent object data
        /// </summary>
        /// <param name="instance">Persistent object to save</param>
        Task SaveObjectAsync(T instance);
        
        /// <summary>
        /// Update the XML document with the persistent object data
        /// </summary>
        /// <param name="instance">Persistent object to save</param>
        void SaveObject(T instance);
        
        /// <summary>
        /// Delete a persistent object from the XML document based on the identifier
        /// </summary>
        /// <param name="id">The identifier of the persistent object to delete</param>
        Task DeleteObjectAsync(Guid id);
        
        /// <summary>
        /// Delete a persistent object from the XML document based on the identifier
        /// </summary>
        /// <param name="id">The identifier of the persistent object to delete</param>
        void DeleteObject(Guid id);
    }
}
