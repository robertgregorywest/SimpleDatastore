using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Interface for mutating persistent objects
    /// </summary>
    /// <typeparam name="T">PersistentObject type to work with</typeparam>
    /// <typeparam name="TKey">Type of the identifier</typeparam>
    public interface IWriteRepository<T, in TKey> 
        where T : PersistentObject<TKey> 
        where TKey : struct
    {
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
        Task DeleteAsync(TKey id);
        
        /// <summary>
        /// Delete a persistent object from the storage document
        /// </summary>
        /// <param name="id">Identifier to delete</param>
        [UsedImplicitly]
        void Delete(TKey id);
    }
}