using System.Threading.Tasks;

namespace SimpleDatastore.Interfaces
{
    /// <summary>
    /// Handles retrieval and saving of the storage documents
    /// </summary>
    /// <typeparam name="T">The persistent object type to work with</typeparam>
    /// <typeparam name="TDocument">The type of the document to return</typeparam>
    /// <typeparam name="TKey">The type of the identifier</typeparam>
    public interface IDocumentProvider<T, TKey, TDocument> 
        where T : PersistentObject<TKey> 
        where TKey : struct
    {
        /// <summary>
        /// Get the document for the type
        /// </summary>
        /// <returns>Document for the persistent object type</returns>
        Task<TDocument> GetDocumentAsync();
        
        /// <summary>
        /// Get the document for the type
        /// </summary>
        /// <returns>Document for the persistent object type</returns>
        TDocument GetDocument();
        
        /// <summary>
        /// Save the storage document for the persistent object type  
        /// </summary>
        /// <param name="document">The document to save</param>
        Task SaveDocumentAsync(TDocument document);
        
        /// <summary>
        /// Save the storage document for the persistent object type  
        /// </summary>
        /// <param name="document">The document to save</param>
        void SaveDocument(TDocument document);
    }
}