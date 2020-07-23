using System.Threading.Tasks;
using System.Text.Json;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Responsible for resolving a PersistentObject from a JsonElement
    /// </summary>
    /// <typeparam name="T">PersistentObject type to resolve</typeparam>
    public interface IItemResolverJson<T> where T : PersistentObject
    {
        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="JsonElement"/>
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> containing the persistent object data</param>
        /// <returns>Hydrated persistent object</returns>
        Task<T> GetItemFromNodeAsync(JsonElement element);
        
        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="JsonElement"/>
        /// </summary>
        /// <param name="element">The <see cref="JsonElement"/> containing the persistent object data</param>
        /// <returns>Hydrated persistent object</returns>
        T GetItemFromNode(JsonElement element);
    }
}