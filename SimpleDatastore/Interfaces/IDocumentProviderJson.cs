using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Handles retrieval and saving of the storage documents
    /// </summary>
    /// <typeparam name="T">The persistent object type to work with</typeparam>
    public interface IDocumentProviderJson<T> where T : PersistentObject
    {
        /// <summary>
        /// Get the document for the type
        /// </summary>
        /// <returns>Document for the persistent object type</returns>
        Task<string> GetDocumentAsync();
        
        /// <summary>
        /// Get the document for the type
        /// </summary>
        /// <returns>Document for the persistent object type</returns>
        string GetDocument();
        
        /// <summary>
        /// Save the storage document for the persistent object type  
        /// </summary>
        /// <param name="document">The document to save</param>
        Task SaveDocumentAsync(string document);
        
        /// <summary>
        /// Save the storage document for the persistent object type  
        /// </summary>
        /// <param name="document">The document to save</param>
        void SaveDocument(string document);
    }
}