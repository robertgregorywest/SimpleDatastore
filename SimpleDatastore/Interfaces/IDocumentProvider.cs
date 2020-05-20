using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Handles retrieval and saving of the storage documents
    /// </summary>
    /// <typeparam name="T">The persistent object type to work with</typeparam>
    public interface IDocumentProvider<T> where T : PersistentObject
    {
        /// <summary>
        /// Get the document for the type
        /// </summary>
        /// <returns>XDocument for the persistent object type</returns>
        Task<XDocument> GetDocumentAsync();
        
        /// <summary>
        /// Save the storage document for the persistent object type  
        /// </summary>
        /// <param name="document">The XDocument to save</param>
        Task SaveDocumentAsync(XDocument document);
    }
}
