using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Interface for reading persistent objects
    /// </summary>
    /// <typeparam name="T">PersistentObject type to work with</typeparam>
    /// <typeparam name="TKey">Type of the identifier</typeparam>
    public interface IReadRepository<T, in TKey> 
        where T : PersistentObject<TKey> 
        where TKey : struct
    {
        /// <summary>
        /// Get a single persistent object
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Persistent object</returns>
        [UsedImplicitly]
        Task<T> LoadAsync(TKey id);
        
        /// <summary>
        /// Get a single persistent object Option
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Option of persistent object</returns>
        [UsedImplicitly]
        Task<Option<T>> LoadOptionAsync(TKey id);
        
        /// <summary>
        /// Get a single persistent object
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Persistent object</returns>
        [UsedImplicitly]
        T Load(TKey id);
        
        /// <summary>
        /// Get a single persistent object Option
        /// </summary>
        /// <param name="id">Identifier for the persistent object</param>
        /// <returns>Option of persistent object</returns>
        [UsedImplicitly]
        Option<T> LoadOption(TKey id);

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
        /// Get persistent objects based on provided identifiers
        /// </summary>
        /// <param name="persistentObjectIds">The array of persistent object identifiers to retrieve</param>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        Task<IList<T>> LoadCollectionByIdsAsync(IEnumerable<string> persistentObjectIds);
        
        /// <summary>
        /// Get persistent objects based on provided identifiers
        /// </summary>
        /// <param name="persistentObjectIds">The array of persistent object identifiers to retrieve</param>
        /// <returns>List of persistent objects</returns>
        [UsedImplicitly]
        IList<T> LoadCollectionByIds(IEnumerable<string> persistentObjectIds);
    }
}