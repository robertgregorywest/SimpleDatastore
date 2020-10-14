using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Responsible for resolving a PersistentObject from an XElement
    /// </summary>
    /// <typeparam name="T">PersistentObject type to resolve</typeparam>
    /// <typeparam name="TElement">Type of the element from which to retrieve the item</typeparam>
    public interface IItemResolver<T, in TElement> where T : PersistentObject
    {
        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="TElement"/>
        /// </summary>
        /// <param name="element">The <see cref="TElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <see cref="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        Task<T> GetItemFromNodeAsync(TElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);

        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="TElement"/>
        /// </summary>
        /// <param name="element">The <see cref="TElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <see cref="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        T GetItemFromNode(TElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);
    }
}