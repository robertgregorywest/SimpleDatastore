using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Responsible for resolving a PersistentObject from an XElement
    /// </summary>
    /// <typeparam name="T">PersistentObject type to resolve</typeparam>
    public interface IItemResolverXml<T> where T : PersistentObject
    {
        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="XElement"/>
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <see cref="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        Task<T> GetItemFromNodeAsync(XElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);

        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="XElement"/>
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> containing the persistent object data</param>
        /// <param name="activator">Func to create instance of <see cref="T"/></param>
        /// <param name="repoProvider">Func to create instances of repositories</param>
        /// <param name="persistChildren">Whether to persist child <see cref="PersistentObject"/> instances locally or in the type storage document</param>
        /// <returns>Hydrated persistent object</returns>
        T GetItemFromNode(XElement element, Func<Type, object> activator, Func<Type, dynamic> repoProvider, bool persistChildren);
    }
}
