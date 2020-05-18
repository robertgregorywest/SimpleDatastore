using System.Threading.Tasks;
using System.Xml;

namespace SimpleDatastore.Interfaces
{
    public interface IDocumentProvider<T> where T : PersistentObject
    {
        Task<XmlDocument> GetDocumentAsync();
        Task SaveDocumentAsync(XmlDocument document);
    }
}
