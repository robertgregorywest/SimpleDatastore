using System;
using System.Threading.Tasks;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Responsible for resolving a PersistentObject from a <typeparamref name="TElement"/>
    /// </summary>
    /// <typeparam name="T">PersistentObject type to resolve</typeparam>
    /// <typeparam name="TElement">Type of the element from which to retrieve the item</typeparam>
    public interface IItemResolver<T, in TElement> where T : PersistentObject
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> from the <typeparamref name="TElement"/>
        /// </summary>
        /// <param name="element">The <typeparamref name="TElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <typeparamref name="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        Task<T> GetItemFromNodeAsync(TElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> from the <typeparamref name="TElement"/>
        /// </summary>
        /// <param name="element">The <typeparamref name="TElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <typeparamref name="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        T GetItemFromNode(TElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);
    }
}