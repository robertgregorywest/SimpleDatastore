using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Responsible for resolving a PersistentObject from an XElement
    /// </summary>
    /// <typeparam name="T">PersistentObject type to resolve</typeparam>
    public interface IItemResolver<T> where T : PersistentObject
    {
        /// <summary>
        /// Creates an instance of <see cref="T"/> from the <see cref="XElement"/>
        /// </summary>
        /// <param name="element">The XElement containing the persistent object data</param>
        /// <returns>Hydrated persistent object</returns>
        Task<T> GetItemFromNodeAsync(XElement element);
    }
}
